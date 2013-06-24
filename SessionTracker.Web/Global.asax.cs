using Autofac;
using Autofac.Integration.WebApi;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using AutoMapper;
using SessionTracker.Web.Entities;
using SessionTracker.Web.Dtos;
using SessionTracker.Web.RequestModels;

namespace SessionTracker.Web {

    public class Global : System.Web.HttpApplication {

        protected void Application_Start(object sender, EventArgs e) {

            // RevenDb ref:
            // Don't fight the fx and store ids as string: http://kijanawoodard.com/just-use-string-id-for-ravendb
            // Using Integer Document IDs in RavenDB Indexes: http://blog.mariusschulz.com/using-integer-document-ids-in-ravendb-indexes
            // RavenDB identity strategy in ASP.NET Web API: http://ben.onfabrik.com/posts/ravendb-identity-strategy-in-aspnet-web-api

            ConfigureAutoMapper();

            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Routes.MapHttpRoute("DefaultHttpRoute", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.DependencyResolver = new AutofacWebApiDependencyResolver(RegisterServices(new ContainerBuilder()));
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

    public static class NumericExtensions {

        public static int ToIntId(this string id) {

            return int.Parse(id.Substring(id.LastIndexOf('/') + 1));
        }
    }

    public static class DocumentSessionExtensions {

        public static string GetStringId<T>(this IDocumentSession session, ValueType id) {

            return session.Advanced.DocumentStore.Conventions
                .DefaultFindFullDocumentKeyFromNonStringIdentifier(id, typeof(T), false);
        }

        public static string GetStringId<T>(this IAsyncDocumentSession session, ValueType id) {

            return session.Advanced.DocumentStore.Conventions
                .DefaultFindFullDocumentKeyFromNonStringIdentifier(id, typeof(T), false);
        }
    }

    public class IntTypeConverter : TypeConverter<string, int> {

        protected override int ConvertCore(string source) {

            if (source == null) {

                throw new AutoMapperMappingException("Cannot convert null string to non-nullable return type.");
            }

            if (source.Contains("/")) {

                return source.ToIntId();
            }

            return int.Parse(source);
        }
    }

    public class NullIntTypeConverter : TypeConverter<string, int?> {

        protected override int? ConvertCore(string source) {

            int? nullableValue = new Nullable<int>();
            if (source != null) {

                if (source.Contains("/")) {

                    nullableValue = source.ToIntId();
                }
                else {

                    int result;
                    if (int.TryParse(source, out result)) {

                        nullableValue = result;
                    }
                }
            }

            return nullableValue;
        }
    }
}