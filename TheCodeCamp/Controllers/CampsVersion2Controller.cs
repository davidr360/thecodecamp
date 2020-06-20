﻿using AutoMapper;
using Microsoft.Web.Http;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/camps")]
    public class CampsVersion2Controller : ApiController
    {
        private readonly ICampRepository _db;
        private readonly IMapper _mapper;

        public CampsVersion2Controller(ICampRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll(bool includeTalks = false)
        {
            try
            {
                var result = await _db.GetAllCampsAsync(includeTalks);
                var mappedResult = _mapper.Map<CampModel[]>(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }            
        }

        [HttpGet]
        [Route("{moniker}", Name = "GetCamp20")]
        public async Task<IHttpActionResult> Get(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await _db.GetCampAsync(moniker, includeTalks);

                if (result == null) return NotFound();

                var mappedResult = _mapper.Map<CampModel>(result);

                return Ok(new { success = true, camp = mappedResult });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("search/{date:datetime}")]
        public async Task<IHttpActionResult> SearchByDate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var result = await _db.GetAllCampsByEventDate(date, includeTalks);
                var mappedResult = _mapper.Map<CampModel[]>(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create(CampModel model)
        {
            try
            {
                if (await _db.GetCampAsync(model.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker already in use");
                }

                if (ModelState.IsValid)
                {
                    var camp = _mapper.Map<Camp>(model);
                    _db.AddCamp(camp);

                    if (await _db.SaveChangesAsync())
                    {
                        var createdCamp = _mapper.Map<CampModel>(camp);
                        return CreatedAtRoute("GetCamp", new { camp.Moniker }, createdCamp);
                    }
                }                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("{moniker}")]
        public async Task<IHttpActionResult> Update(string moniker, CampModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = await _db.GetCampAsync(moniker);
                    if (camp == null) return NotFound();

                    _mapper.Map(model, camp);

                    if (await _db.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<CampModel>(camp));
                    }
                    else
                    {
                        return InternalServerError();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("{moniker}")]
        public async Task<IHttpActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await _db.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _db.DeleteCamp(camp);

                if (await _db.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
