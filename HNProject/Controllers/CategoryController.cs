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
    [RoutePrefix("Categories")]
    public class CategoryController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.Categories.Select(x => new
            {
                x.id_cate,
                x.name,
                x.url_icon,
                x.url_image
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
                var cate = _context.Categories.Where(x => x.id_cate == id).FirstOrDefault();
                if (cate == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_cate = cate.id_cate,
                    name = cate.name,
                    url_icon = cate.url_icon,
                    url_image = cate.url_image
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(CategoryVM model)
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
                var cate = _context.Categories.Add(new Models.Category
                {
                    id_cate = model.id_cate,
                    name = model.name,
                    url_icon = model.url_icon,
                    url_image = model.url_image
                });
                _context.SaveChanges();
                return Ok(cate);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id, CategoryVM model)
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
                var cate = _context.Entry(new Category
                {
                    id_cate = id,
                    name = model.name,
                    url_icon = model.url_icon,
                    url_image = model.url_image
                }).State = System.Data.Entity.EntityState.Modified;
                return Ok(cate);
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
                var cate = _context.Entry(new Category
                {
                    id_cate = id
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
