using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("GroupAddress")]
    public class GroupAddressController : BaseController
    {
        public IHttpActionResult Post(GroupAddressVM model)
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
                var group_address = _context.GroupAddresses.Add(new Models.GroupAddress
                {
                    id_group_address = model.id_group_address
                });
                _context.SaveChanges();
                return Ok(group_address);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}