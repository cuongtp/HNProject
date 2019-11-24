
using HNProject.Models;
using HNProject.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Accounts")]
    public class AccountController : BaseController
    {

        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.Accounts.Select(x => new
            {
                x.id_account,
                x.username,
                x.password,
                x.phone,
                x.name,
                x.dob,
                x.email,
                x.fb_id,
                Address = x.GroupAddress.Addresses.Select(_ => _.address1),
                groupImages = x.GroupImage.Images.Select(_ => _.url),
                roleName = x.Role.name,
                order_id = x.Orders.Select(_ => _.id_order),
                fcm_token = x.fcm_token,
                connection_id = x.connection_id,
                isSelected =x.isSelected,
                favouriteProducts = x.FavouriteProducts.Select(_ => _.Product.name),
                levels = x.Levels.Select(_ => _.point),
                x.state,

            }));
        }

        [HttpGet, Route("idAccount_getAddress")]
        public IHttpActionResult GetAddressByAccount(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var address = _context.Accounts.FirstOrDefault(x => x.id_account == id).GroupAddress.Addresses;
                if (address == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                   address =  address.Select(x => new
                    {
                        x.id_address,
                        x.address1,
                        x.lat,
                        x.@long,
                        
                    })
                }) ;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
                var account = _context.Accounts.Where(x => x.id_account == id).FirstOrDefault();
                if (account == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id_account = account.id_account,
                    username = account.username,
                    password = account.password,
                    phone = account.phone,
                    name = account.name,
                    dob = account.dob,
                    email = account.email,
                    fb_id = account.fb_id,
                    fcm_token = account.fcm_token,
                    connection_id = account.connection_id,
                    isSelected = account.isSelected,
                    groupAddress = account.GroupAddress.Addresses.Select(_ => _.address1),
                    groupImages = account.GroupImage.Images.Select(x => x.url),
                    roleName = account.Role.name,
                    favouriteProducts = account.FavouriteProducts.Select(_ => _.Product.name),
                    levels = account.Levels.Select(_ => _.point),
                    account.state,
                }); ;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost, Route("")]
        public IHttpActionResult Post(AccountVM model)
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
                var product = _context.Accounts.Add(new Account
                {
                    id_account = model.id_account,
                    username = model.username,
                    password = model.password,
                    phone = model.phone,
                    name = model.name,
                    dob = model.dob,
                    email = model.email,
                    fb_id = model.fb_id,
                    fcm_token = model.fcm_token,
                    connection_id = model.connection_id,
                    id_group_address = model.id_group_address,
                    id_group_image = model.id_group_image,
                    id_role = model.id_role,
                    state = model.state,
                    isSelected = model.isSelected,
                });
                _context.SaveChanges();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPut, Route("id_account_update_FCM")]
        public IHttpActionResult UpdateFCM(string id,string fcm)
        {
            try
            {
                var account = _context.Accounts.Find(id);
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (account == null)
                {
                    return NotFound();
                }

                account.fcm_token = fcm;
                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("id_account_update_Point")]
        public IHttpActionResult Update_point(string id_account, int point)
        {
            try
            {
                var level = _context.Levels.Where(x => x.id_account == id_account).FirstOrDefault();
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (level == null)
                {
                    return NotFound();
                }

                level.point = level.point+point;
                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("id_account_dpdate_ConnectionID")]
        public IHttpActionResult UpdateConnectionID(string id, string connection_id)
        {
            try
            {
                var account = _context.Accounts.Find(id);
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (account == null)
                {
                    return NotFound();
                }

                account.connection_id = connection_id;
                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


        [HttpPut, Route("")]
        public IHttpActionResult Put(string id, AccountVM model)
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
                var account = _context.Entry(new Account
                {
                    id_account = id,
                    username = model.username,
                    password = model.password,
                    phone = model.phone,
                    name = model.name,
                    dob = model.dob,
                    email = model.email,
                    fb_id = model.fb_id,
                    fcm_token = model.fcm_token,
                    id_group_address = model.id_group_address,
                    id_group_image = model.id_group_image,
                    id_role = model.id_role,
                    state = model.state,
                    connection_id = model.connection_id,
                    isSelected = model.isSelected,
                }).State = EntityState.Modified;

                _context.SaveChanges();

                return Ok(new { });
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
                var account = _context.Entry(new Account
                {
                    id_account = id
                }).State = EntityState.Deleted;
                _context.SaveChanges();
                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
