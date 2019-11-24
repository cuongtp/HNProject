using HNProject.Service;
using HNProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HNProject.Controllers
{
    [RoutePrefix("Logins")]
    public class LoginController : BaseController
    {
        [HttpPost, Route("")]
        public IHttpActionResult Login(LoginVM loginVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var model = _context.Accounts.FirstOrDefault(x => x.username == loginVM.username && x.password == loginVM.password);
                if (model == null)
                {
                    return NotFound();
                }



                var start = DateTime.Now;
                var id_account = model.id_account;
                var expire_day = start.AddDays(10);
                var object_login = new
                {
                    name = model.name,
                    phone = model.phone,
                    id = model.id_account,
                    expire_day = expire_day,
                    id_group_image = model.id_group_image 
                };

                var json = new JavaScriptSerializer().Serialize(object_login);
                string string_json = new JavaScriptSerializer().Serialize(object_login);

                byte[] data_string_json = System.Text.ASCIIEncoding.ASCII.GetBytes(string_json);




                var buildBase64 = data_string_json;

                var A = System.Convert.ToBase64String(buildBase64);
                var key = "teamsida";
                //key = BuildHash256.HexDecode(key);

                var B = BuildHash256.CalcHMACSHA256Hash(A.ToString(), key);

                var token = string.Join("", A.Concat(".").Concat(B));
                var url = "";
                if(model.GroupImage != null)
                {
                    url = model.GroupImage.Images.Select(x => x.url).FirstOrDefault();
                }

                return Ok(new
                {
                    id = model.id_account,
                    username = model.username,
                    password = model.password,
                    token = token,
                    name = model.name,
                    phone = model.phone,
                    img_url = url,
                    email = model.email
                });
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


        }
    }
}
