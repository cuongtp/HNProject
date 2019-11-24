using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Levels")]
    public class LevelController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.Levels.Select(x => new
            {
                x.id_account,
                x.id_role,
                x.point
            }));
        }

        [HttpGet, Route("id")]
        public IHttpActionResult GetById(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var level = _context.Levels.Where(x => x.id_account == id).FirstOrDefault();
                if (level == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_product = level.id_account,
                    id_role = level.id_role,
                    point = level.point
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(LevelVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model == null)
                {
                    return NotFound();
                }
                var level = _context.Levels.Add(new Models.Level
                {
                    id_account = model.id_account,
                    id_role = model.id_role,
                    point = model.point
                });
                _context.SaveChanges();
                return Ok(level);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id, LevelVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model == null)
                {
                    return NotFound();
                }
                var level = _context.Entry(new Models.Level
                {
                    id_account = id,
                    id_role = model.id_role,
                    point = model.point
                }).State = System.Data.Entity.EntityState.Modified;
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route("")]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var level = _context.Entry(new Models.Level
                {
                    id_account = id
                }).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
