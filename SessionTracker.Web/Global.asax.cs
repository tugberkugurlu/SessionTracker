using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using SessionTracker.Web.Dtos;
using SessionTracker.Web.Entities;
using SessionTracker.Web.Infrastructure;
using SessionTracker.Web.Infrastructure.Formatting;
using SessionTracker.Web.Infrastructure.Indexes;
using SessionTracker.Web.MessageHandlers;
using SessionTracker.Web.RequestModels;
using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using WebApiDoodle.Web.Filters;

namespace SessionTracker.Web {

    public class Global : System.Web.HttpApplication {

        protected void Application_Start(object sender, EventArgs e) {

            // RevenDb ref:
            // Don't fight the fx and store ids as string: http://kijanawoodard.com/just-use-string-id-for-ravendb
            // Using Integer Document IDs in RavenDB Indexes: http://blog.mariusschulz.com/using-integer-document-ids-in-ravendb-indexes
            // RavenDB identity strategy in ASP.NET Web API: http://ben.onfabrik.com/posts/ravendb-identity-strategy-in-aspnet-web-api

            ConfigureAutoMapper();

            HttpConfiguration config = GlobalConfiguration.Configuration;
            IContainer container = RegisterServices(new ContainerBuilder());
            config.Routes.MapHttpRoute("DefaultHttpRoute", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Services.RemoveAll(typeof(ModelValidatorProvider), validator => !(validator is DataAnnotationsModelValidatorProvider));
            config.Filters.Add(new InvalidModelStateFilterAttribute());
            config.MessageHandlers.Add(new CorrelationHandler());

            ConfigureFormatters(config.Formatters);

            // Create RavenDB indexes
            IDocumentStore documentStore = container.Resolve<IDocumentStore>();
            IndexCreation.CreateIndexes(typeof(Tags_Count).Assembly, documentStore);
        }

        private static void ConfigureFormatters(MediaTypeFormatterCollection formatters)
        {
            // Remove unnecessary formatters
            MediaTypeFormatter jqueryFormatter = formatters.FirstOrDefault(x => x.GetType() == typeof(JQueryMvcFormUrlEncodedFormatter));
            formatters.Remove(formatters.XmlFormatter);
            formatters.Remove(formatters.FormUrlEncodedFormatter);
            formatters.Remove(jqueryFormatter);

            // Suppressing the IRequiredMemberSelector for all formatters
            foreach (MediaTypeFormatter formatter in formatters)
            {
                formatter.RequiredMemberSelector = new SuppressedRequiredMemberSelector();
            }

            formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private IContainer RegisterServices(ContainerBuilder builder) {

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => 
                {
                    const string DefaultDatabase = "TechSessions";
                    IDocumentStore store = new DocumentStore {
                        Url = "http://localhost:8080",
                        DefaultDatabase = DefaultDatabase }.Initialize();

                    store.DatabaseCommands.EnsureDatabaseExists(DefaultDatabase);
                    return store;

                }).As<IDocumentStore>().SingleInstance();

            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession())
                   .As<IAsyncDocumentSession>()
                   .InstancePerApiRequest();

            builder.Register(c => Mapper.Engine)
                   .As<IMappingEngine>()
                   .SingleInstance();

            return builder.Build();
        }

        private void ConfigureAutoMapper() {

            // For mapping ravendb string ids to integer
            Mapper.CreateMap<string, int>().ConvertUsing(new IntTypeConverter());
            Mapper.CreateMap<string, int?>().ConvertUsing(new NullIntTypeConverter());

            Mapper.CreateMap<Speaker, SpeakerDto>();
            Mapper.CreateMap<Session, SessionDto>();
            Mapper.CreateMap<SpeakerRequestModel, Speaker>();
            Mapper.CreateMap<SessionRequestModel, Session>();
        }
    }
}