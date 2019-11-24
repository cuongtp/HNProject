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
    [RoutePrefix("UpdateOrder")]
    public class UpdateOrderController : BaseController
    {

        [HttpPut, Route("Update_State")]
        public async System.Threading.Tasks.Task<IHttpActionResult> PutAsync(string id_order, int new_state)
        {
            try
            {
                var order = _context.Orders.Find(id_order);
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                if (order == null)
                {
                    return NotFound();
                }

                //order.state = 2;
                order.state = new_state;

                if(new_state == 8)
                {
                    var shipper = _context.Accounts.Find(order.id_shipper);
                    shipper.isSelected = 0;
                    shipper.state = 0;
                    _context.SaveChanges();
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Đơn hàng của bạn đã bị hủy",
                        id_order = id_order,
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
                }
                if (new_state == 7)
                {
                    var shipper = _context.Accounts.Find(order.id_shipper);
                    shipper.isSelected = 0;
                    shipper.state = 0;
                    _context.SaveChanges();
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Đơn hàng của bạn đã hoàn tất",
                        id_order = id_order,
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
                }
                if (new_state == 6)
                {
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Đơn hàng của bạn đã đến nơi",
                        id_order = id_order,
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
                }
                if (new_state == 4)
                {
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Đơn hàng của bạn đã được mua!",
                        id_order = id_order,
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
                }
                if (new_state == 3)
                {
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Shipper đã đến siêu thị để mua hàng!",
                        id_order = id_order,
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
                }
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
