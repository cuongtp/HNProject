using HNProject.Models;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("FeedBacks")]
    public class FeedBackController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.FeedBacks.Select(x => new
            {
                x.id_feedback,
                orders = x.Order,
                x.feedback_content,
                x.feedback_rate
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
                var feedBack = _context.FeedBacks.Where(x => x.id_feedback == id).FirstOrDefault();
                if (feedBack == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_feedback = feedBack.id_feedback,
                    orders = feedBack.Order,
                    feedBack_content = feedBack.feedback_content,
                    feedBack_rate = feedBack.feedback_rate
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(FeedBackVM model)
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
                var feedBack = _context.FeedBacks.Add(new Models.FeedBack
                {
                    id_feedback = model.id_feedback,
                    id_order = model.id_order,
                    id_role = model.id_role,
                    feedback_content = model.feedback_content,
                    feedback_rate = model.feedback_rate
                });
                _context.SaveChanges();
                return Ok(feedBack);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id,FeedBackVM model)
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
                var feedBack = _context.Entry(new FeedBack
                {
                    id_feedback = id,
                    id_order = model.id_order,
                    id_role = model.id_role,
                    feedback_content = model.feedback_content,
                    feedback_rate = model.feedback_rate
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

                var feedBack = _context.Entry(new FeedBack
                {
                    id_feedback = id
                }).State = System.Data.Entity.EntityState.Deleted;
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
