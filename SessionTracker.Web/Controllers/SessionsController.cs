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
    
    public class SessionsController : RavenApiController {

        private readonly IMappingEngine _mapper;

        public SessionsController(IAsyncDocumentSession documentSession, IMappingEngine mapper) 
            : base(documentSession) {

            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionDto>> GetSessions() {

            IEnumerable<Session> sessions = await RavenSession
                .Query<Session>()
                .Include<Session>(ses => ses.SpeakerId)
                .ToListAsync();

            IEnumerable<Task<SessionDto>> sessionDtoTasks = _mapper
                .Map<IEnumerable<Session>, IEnumerable<SessionDto>>(sessions)
                .Select(async sesDto => 
                {
                    // Note: As we have included the Speaker for every session,
                    //       we won't make a trip to db for each session
                    Speaker speaker = await RavenSession.LoadAsync<Speaker>(RavenSession.GetStringId<Speaker>(sesDto.SpeakerId));
                    sesDto.Speaker = _mapper.Map<Speaker, SpeakerDto>(speaker);
                    return sesDto;
                });

            await Task.WhenAll(sessionDtoTasks);
            return sessionDtoTasks.Select(x => x.Result);
        }

        public async Task<SessionDto> GetSession(int id) {

            Session session = await RavenSession
                .Include<Session>(ses => ses.SpeakerId)
                .LoadAsync<Session>(id);

            if (session == null) {

                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            Speaker speaker = await RavenSession.LoadAsync<Speaker>(session.SpeakerId);
            SessionDto sessionDto = _mapper.Map<Session, SessionDto>(session);
            sessionDto.Speaker = _mapper.Map<Speaker, SpeakerDto>(speaker);

            return sessionDto;
        }

        public async Task<HttpResponseMessage> PostSession(SessionRequestModel requestModel) {

            Session session = _mapper.Map<SessionRequestModel, Session>(requestModel);
            session.SpeakerId = RavenSession.GetStringId<Speaker>(requestModel.SpeakerId);
            session.CreatedOn = DateTimeOffset.Now;
            session.LastUpdatedOn = DateTimeOffset.Now;

            await RavenSession.StoreAsync(session);
            SessionDto sessionDto = _mapper.Map<Session, SessionDto>(session);

            return Request.CreateResponse(HttpStatusCode.Created, sessionDto);
        }
    }
}