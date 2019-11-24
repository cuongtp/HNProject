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
using System.Web.Script.Serialization;

namespace HNProject.Controllers
{
    [RoutePrefix("OrdersGetFull")]
    public class OrdersGetFullController : BaseController
    {


        [HttpGet, Route("")]
        public IHttpActionResult GetFull()
        {
            //var customer = _context.Orders.Select(x => x.Account);

            //var shipper = _context.Orders.Select(x => x.Account1); ;

            var model = _context.Orders.Select(_ => new 
            {
                id_order = _.id_order,
                created_date = _.created_date,
                id_customer = _.id_customer,
                id_shipper = _.id_shipper,
                state = _.state,
                id_group_image = _.id_group_image,
                id_address = _.id_address,
                point = _.point,
                shipping_cost = _.shipping_cost,
                time_to_ship = _.time_to_ship,
                customer_address_id = _.customer_address_id,
                dis_cus_to_market = _.dis_cus_to_market,
                phone = _.phone,
                name = _.name,
                taking_time = _.taking_time,
                done_time = _.done_time,
                order_code = _.order_code,
                customer_comment = _.customer_comment,
                system_cost = _.system_cost,
                total_amount = _.total_amount,
                token = _.token,
                OrderDetails = _.OrderDetails.Select(__ => new 
                {
                    id_orderdetail = __.id_orderdetail,
                    id_order = __.id_order,
                    id_product = __.id_product,
                    id_market = __.id_market,
                    price = __.price,
                    quanlity = __.quanlity,
                    priority = __.priority,

                }).ToList()
            });

            return Ok(model);
        }

        [HttpGet, Route("id")]
        public IHttpActionResult GetById(string id)
        {
            var customer = _context.Orders.Select(x => x.Account);

            var shipper = _context.Orders.Select(x => x.Account1); ;
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var order = _context.Orders.Find(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_order = order.id_order,
                    created_date = order.created_date,
                    id_customer = order.id_customer,
                    id_shipper = order.id_shipper,
                    state = order.state,
                    id_group_image = order.id_group_image,
                    id_address = order.id_address,
                    point = order.point,
                    shipping_cost = order.shipping_cost,
                    time_to_ship = order.time_to_ship,
                    customer_address_id = order.customer_address_id,
                    dis_cus_to_market = order.dis_cus_to_market,
                    phone = order.phone,
                    name = order.name,
                    taking_time = order.taking_time,
                    done_time = order.done_time,
                    order_code = order.order_code,
                    customer_comment = order.customer_comment,
                    system_cost = order.system_cost,
                    total_amount = order.total_amount,
                    token = order.token,
                    OrderDetails = order.OrderDetails.Select(__ => new
                    {
                        id_orderdetail = __.id_orderdetail,
                        id_order = __.id_order,
                        id_product = __.id_product,
                        id_market = __.id_market,
                        price = __.price,
                        quanlity = __.quanlity,
                        priority = __.priority,

                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("Id_Shipper_Add_to_Order")]
        public async System.Threading.Tasks.Task<IHttpActionResult> PutAsync(string order_id, string id_shipper)
        {
           
            try
            {
                var order = _context.Orders.Find(order_id);
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (order == null)
                {
                    return NotFound();
                }

                //order.state = 2;
                var accoutShipper = _context.Accounts.Find(id_shipper);
                accoutShipper.isSelected = 0;
                accoutShipper.state = 0;
                //order.id_shipper = id_shipper;
                _context.SaveChanges();

                HubConnection hubConnection = new HubConnection("https://localhost:44333");

                IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");
                var shipper = _context.Accounts.Find(id_shipper);

                var json_object = new
                {
                    msg = "Đã tìm thấy shipper: " + shipper.name + " SDT: " + shipper.phone,
                    id_order = order_id,
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var Json_str = serializer.Serialize(json_object);
                string msg = Json_str;

                Account customer = _context.Accounts.Find(order.id_customer);
                var connectionID_cus = customer.connection_id;
                await hubConnection.Start();

                var change = hubConnection.State;
                var connection_id = hubConnection.ConnectionId;
                //while (hubConnection.ConnectionId == "" || hubConnection.ConnectionId == null) { }
                //var conn_id = hubProxy.Invoke("getConnectionId");
                await hubProxy.Invoke("NotifyCustomer", connectionID_cus, msg);

                return Ok(new { });
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


    }
}
