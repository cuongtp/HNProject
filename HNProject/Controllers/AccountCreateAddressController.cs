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
    [RoutePrefix("Account_Create_AddressController")]
    public class Account_Create_AddressController : BaseController
    {
        [HttpPost, Route("id_account")]
        public IHttpActionResult Post(AddressVM model, string id_account)
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
                var account = _context.Accounts.Find(id_account);

                var address = _context.Addresses.Add(new Models.Address
                {
                    id_address = model.id_address,
                    @long = model.@long,
                    lat = model.lat,
                    address1 = model.address1,
                    description = model.description,
                    id_group_address = account.id_group_address,
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

        [HttpPut, Route("Update_Pass_State_isSelected")]
        public IHttpActionResult Put(string id_account, string password, int state, int isSelected)
        {
            var account = _context.Accounts.Find(id_account);
            try
            {
                if (account == null)
                {
                    return NotFound();
                }
                account.password = password;
                account.state = state;
                account.isSelected = isSelected;

                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("Update_name_dob_password")]
        public IHttpActionResult Update_name_dob_password(string id_account, string name, DateTime dob, string password = null)
        {
            var account = _context.Accounts.Find(id_account);
            if(password != null)
            {
                account.password = password;
            }
            
            try
            {
                if (account == null)
                {
                    return NotFound();
                }
                account.name = name;
                account.dob = dob;
                

                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpPut, Route("update-address")]
        public IHttpActionResult Update_address(string id, CMMAddressVM model)
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
                var acount = _context.Accounts.Find(id);

                if (acount == null)
                {
                    return NotFound();
                }

                byte[] buffer = Guid.NewGuid().ToByteArray();
                var id_address = BitConverter.ToInt64(buffer, 0).ToString();

                byte[] buffer2 = Guid.NewGuid().ToByteArray();
                var id_group_address = BitConverter.ToInt64(buffer2, 0).ToString();

                var query = acount.GroupAddress.Addresses.FirstOrDefault();
                var gaddress = acount.id_group_address;
                _context.Addresses.Remove(query);
                acount.GroupAddress.Addresses = model.Addresses.Select(x => new Address
                {
                    id_address = id_address,
                    id_group_address = gaddress,
                    address1 = x.Address,
                    description = x.Description,
                    lat = x.Lat,
                    @long = x.Long
                }).ToList();
           
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
