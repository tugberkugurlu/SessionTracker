using AutoMapper;
using Raven.Client;
using SessionTracker.Web.Dtos;
using SessionTracker.Web.Entities;
using SessionTracker.Web.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SessionTracker.Web.Controllers {

    public class SpeakersController : RavenApiController {

        private readonly IMappingEngine _mapper;

        public SpeakersController(IAsyncDocumentSession documentSession, IMappingEngine mapper) 
            : base(documentSession) {

            _mapper = mapper;
        }

        public async Task<IEnumerable<SpeakerDto>> GetSpeakers() {

            IList<Speaker> speakers = await RavenSession.Query<Speaker>().ToListAsync();
            return _mapper.Map<IEnumerable<Speaker>, IEnumerable<SpeakerDto>>(speakers);
        }

        public async Task<SpeakerDto> GetSpeaker(int id) {

            Speaker speaker = await RavenSession.LoadAsync<Speaker>(id);

            if (speaker == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return _mapper.Map<Speaker, SpeakerDto>(speaker);
        }

        public async Task<HttpResponseMessage> PostSpeaker(SpeakerRequestModel requestModel) {

            Speaker speaker = _mapper.Map<SpeakerRequestModel, Speaker>(requestModel);
            speaker.CreatedOn = DateTimeOffset.Now;
            speaker.LastUpdatedOn = DateTimeOffset.Now;

            await RavenSession.StoreAsync(speaker);
            SpeakerDto speakerDto = _mapper.Map<Speaker, SpeakerDto>(speaker);

            return Request.CreateResponse(HttpStatusCode.Created, speakerDto);
        }
    }
}