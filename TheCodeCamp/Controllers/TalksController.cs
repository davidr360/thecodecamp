using AutoMapper;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _db;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll(string moniker, bool includeSpeakers = false)
        {
            var result = await _db.GetTalksByMonikerAsync(moniker, includeSpeakers);

            return Ok(_mapper.Map<TalkModel[]>(result));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetTalkById")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeaker = false)
        {
            var result = await _db.GetTalkByMonikerAsync(moniker, id, includeSpeaker);
            if (result == null) return NotFound();

            return Ok(_mapper.Map<TalkModel>(result));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Add(string moniker, TalkModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var camp = await _db.GetCampAsync(moniker);
            if (camp == null) return NotFound();

            var talk = _mapper.Map<Talk>(model);
            camp.Talks.Add(talk);

            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetTalkById", new { moniker, Id = talk.TalkId }, _mapper.Map(talk, model));
        }

        [HttpDelete]
        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> DeleteTalk(string moniker, int talkId)
        {
            var talk = await _db.GetTalkByMonikerAsync(moniker, talkId);
            if (talk == null) return NotFound();

            _db.DeleteTalk(talk);

            await _db.SaveChangesAsync();

            return Ok();
        }

        /*
         * HACK: Delete methods are supposed to return a 204 No Content, but Web API 2 doesn't have an ActionResult built in for this
         * You can make your own!
         */
    }
}
