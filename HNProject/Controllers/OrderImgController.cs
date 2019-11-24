using HNProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;
using HNProject.ViewModels;
using Microsoft.AspNet.SignalR.Client;
using HNProject.Service;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;

namespace HNProject.Controllers
{
    [RoutePrefix("OrderImg")]
    public class OrderImgController : BaseController
    {

        [HttpPut, Route("id_order")]
        public IHttpActionResult Put([FromBody]UrlVM url, [FromUri]string id_order)
        {
            var order = _context.Orders.Find(id_order);

            if(order == null)
            {
                return NotFound();
            }
            var group_img = order.GroupImage;
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var group_img_id = BitConverter.ToInt64(buffer, 0).ToString();

            byte[] buffer1 = Guid.NewGuid().ToByteArray();
            var img_id = BitConverter.ToInt64(buffer1, 0).ToString();
            try
            {
                if (group_img == null)
                {
                    _context.GroupImages.Add(new GroupImage
                    {
                        id_group = group_img_id
                        
                    });
                    order.id_group_image = group_img_id;

                    _context.Images.Add(new Image
                    {
                        id_group = group_img_id,
                        name = "order_img",
                        id_image = img_id,
                        url = url.url,
                        describe = "Order IMG"
                        
                    }) ;
                }
                else
                {
                    order.GroupImage.Images.FirstOrDefault().url = url.url;
                }

                

                _context.SaveChanges();

                

               
                return Ok(url);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet, Route("GetOrderImgURL")]
        public IHttpActionResult GetById(string id_order)
        {
            try {
                var order = _context.Orders.Find(id_order);
                if(order == null)
                {
                    return NotFound();
                }
                var url = "";
                if (order.GroupImage != null)
                {
                    url = order.GroupImage.Images.Select(x => x.url).FirstOrDefault();
                }
                return Ok(url);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

 
    }
}
