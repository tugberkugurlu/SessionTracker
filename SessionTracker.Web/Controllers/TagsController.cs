using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Raven.Client;
using SessionTracker.Web.Infrastructure.Indexes;

namespace SessionTracker.Web.Controllers
{
    public class TagsController : RavenApiController
    {
        private readonly IMappingEngine _mapper;

        public TagsController(IAsyncDocumentSession documentSession, IMappingEngine mapper) : base(documentSession)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Tags_Count.ReduceResult>> GetTags()
        {
            IList<Tags_Count.ReduceResult> tags = await RavenSession
                .Query<Tags_Count.ReduceResult, Tags_Count>()
                .OrderByDescending(tag => tag.Count)
                .ToListAsync();

            return tags;
        }
    }
}