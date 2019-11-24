using HNProject.Models;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Addresses")]
    public class AddressController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.Addresses.Select(x => new
            {
                x.id_address,
                x.@long,
                x.lat,
                x.address1,
                x.id_group_address,
                x.description,
                x.priority,
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
                var address = _context.Addresses.Where(x => x.id_address == id).FirstOrDefault();
                if (address == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_address = address.id_address,
                    longtitute = address.@long,
                    latitute = address.lat,
                    address = address.address1,
                    id_group_address = address.id_group_address,
                    description = address.description,
                    priority = address.priority,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Post(AddressVM model)
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

                var address = _context.Addresses.Add(new Models.Address
                {
                    id_address = model.id_address,
                    @long = model.@long,
                    lat = model.lat,
                    address1 = model.address1,
                    description = model.description,
                    id_group_address = model.id_group_address,
                    priority = model.priority
                });
                _context.SaveChanges();
                return Ok(address);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("")]
        public IHttpActionResult Put(string id, AddressVM model)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var address = _context.Entry(new Address
                {
                    id_address = id,
                    @long = model.@long,
                    lat = model.lat,
                    address1 = model.address1,
                    description = model.description,
                    id_group_address = model.id_group_address,
                    priority = model.priority
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
                var order = _context.Entry(new Address
                {
                    id_address = id
                }).State = EntityState.Deleted;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
