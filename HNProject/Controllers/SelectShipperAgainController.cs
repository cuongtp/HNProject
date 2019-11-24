using HNProject.Models;
using HNProject.ViewModels;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HNProject.Controllers
{
    [RoutePrefix("SelectShipper_Again")]
    public class SelectShipperAgainController : BaseController
    {
        [HttpGet, Route("id_order")]
        public async System.Threading.Tasks.Task<IHttpActionResult> GetAsync(int count,[FromUri]double latitude, [FromUri]double longtitude, string id_order,string id_shipper)
        {
            var accoutShipper = _context.Accounts.Find(id_shipper);
            accoutShipper.state = 0;
            accoutShipper.isSelected = 0;
            _context.SaveChanges();
            count += 1;
            if(count == 3)
            {

                HubConnection hubConnection = new HubConnection("http://localhost:44333");

                IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");

                var __order = _context.Orders.Find(id_order);
                var json_object = new
                {
                    msg = "Không tìm thấy Shipper!",
                    id_order = id_order,
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var Json_str = serializer.Serialize(json_object);
                string msg = Json_str;
                Account _customer = _context.Accounts.Find(__order.id_customer);
                var connectionID_cus = _customer.connection_id;
                await hubConnection.Start();

                var change = hubConnection.State;
                var connection_id = hubConnection.ConnectionId;
                await hubProxy.Invoke("NotifyCustomer", connectionID_cus, msg);
                return NotFound();
            }
            try
            {
                //var order = _context.Orders.Find(id_order);
                var sCoord = new GeoCoordinate(latitude, longtitude);
                List<Order> list_order_true = new List<Order>();
                var order_has_shipper = _context.Orders.Where(x => x.id_shipper != null).ToList();
                foreach (var item in order_has_shipper)
                {
                    if (CheckShipper_has_Order(item.id_order, id_order, latitude, longtitude) == true)
                    {
                        list_order_true.Add(item);
                    }
                }

                var list_shipper_has_order_true = new List<ShipperVM>();
                foreach (var item in list_order_true)
                {
                    list_shipper_has_order_true = _context.Accounts.AsEnumerable().Where(x => x.id_account == item.id_shipper && x.isSelected == 0 && x.connection_id != "0" && x.state == 0).Select(x => new ShipperVM
                    {
                        id = x.id_account,
                        username = x.username,
                        lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                        @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                        levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                        fcm_token = x.fcm_token,
                    }).ToList();


                }

                var list_shipper_has_order = list_shipper_has_order_true.GroupBy(x => x.id).Select(y => y.First());

                var address = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.isSelected == 0 && x.connection_id != "0" && x.state == 0).Select(x => new ShipperVM
                {
                    id = x.id_account,
                    username = x.username,
                    lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                    @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                    levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                    fcm_token = x.fcm_token,
                    id_order = id_order,
                    getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0, x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0))
                }).ToList().OrderBy(x => x.getDistance);

                var list = new List<ShipperVM>();
                var list1 = new List<ShipperVM>();
                var list2 = new List<ShipperVM>();
                foreach (var item in address)
                {
                    if (item.getDistance <= 10000)
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in list_shipper_has_order)
                {
                    if (list.Contains(item) != true)
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in list.OrderBy(x => x.levels).Take(5))
                {
                    list1.Add(item);
                }


                if (id_shipper != null)
                {
                    var temp = list1.FirstOrDefault(x => x.id == id_shipper);
                    if (temp != null)
                    {
                        list1.Remove(temp);
                    }
                }




                //if (id_shipper != null)
                //{
                //    var s = _context.Accounts.FirstOrDefault(x => x.id_account == id_shipper);
                //    var _shipper = new ShipperVM
                //    {
                //        id = s.id_account,
                //        username = s.username,
                //        lat = s.GroupAddress.Addresses.FirstOrDefault().lat,
                //        @long = s.GroupAddress.Addresses.FirstOrDefault().@long,
                //        id_order = id_order,
                //        levels = s.Levels.FirstOrDefault().point,
                //        getDistance = sCoord.GetDistanceTo(new GeoCoordinate(s.GroupAddress.Addresses.FirstOrDefault().@long ?? 0, s.GroupAddress.Addresses.FirstOrDefault().lat ?? 0))
                //    };

                //}
                if (list1.Count ==0)
                {

                    var orrder = _context.Orders.Find(id_order);
                    HubConnection hubConnection = new HubConnection("http://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Không tìm thấy Shipper!",
                        id_order = id_order,
                    };

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var Json_str = serializer.Serialize(json_object);
                    string msg = Json_str;
                    Account _customer = _context.Accounts.Find(orrder.id_customer);
                    var connectionID_cus = _customer.connection_id;
                    await hubConnection.Start();

                    var change = hubConnection.State;
                    var connection_id = hubConnection.ConnectionId;
                    await hubProxy.Invoke("NotifyCustomer", connectionID_cus, msg);
                }
                else 
                {
                    list1.Reverse();
                    HubConnection hubConnection = new HubConnection("http://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");

                    string msg = "Bạn có một đơn hàng!";

                    ShipperVM shipper = list1.First();
                    var connectionID_shipper = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.id_account == shipper.id).Select(x => x.connection_id).FirstOrDefault();
                    await hubConnection.Start();

                    var change = hubConnection.State;
                    var connection_id = hubConnection.ConnectionId;
                    //while (hubConnection.ConnectionId == "" || hubConnection.ConnectionId == null) { }
                    //var conn_id = hubProxy.Invoke("getConnectionId");
                    await hubProxy.Invoke("NotifyShipper", connectionID_shipper, id_order, count, msg, shipper.id, latitude, longtitude);


                    //WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    //tRequest.Method = "post";
                    ////serverKey - Key from Firebase cloud messaging server  
                    //tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyDN6yImcd7eWPRiN86cn1C7iecVNaF945M"));
                    ////Sender Id - From firebase project setting  
                    //tRequest.Headers.Add(string.Format("Sender: id={0}", "462404511537"));
                    //tRequest.ContentType = "application/json";


                    //var fcm_s = "eg8nUQs-lbQ:APA91bHPgVKmMy8d0vnpt-u0UMJPivSvU9kaGPyttY1-ZP-gcu6qkt7VuJ9MG-uDPOPCIkDxpgWJ0TfdP_axotwn-mZEcBOpkEXpn1QfD_HeueUMt-myWeS3ZATw_R11s5D2-N83Sq6U";

                    //ShipperVM shipper = list1.First();

                    //fcm_s = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.id_account == shipper.id).Select(x => x.fcm_token).FirstOrDefault();
                    //var payload = new
                    //{
                    //    to = fcm_s,
                    //    priority = "high",
                    //    content_available = true,
                    //    notification = new
                    //    {
                    //        body = "Test",
                    //        title = "Test",
                    //        badge = 1
                    //    },

                    //};

                    //string postbody = JsonConvert.SerializeObject(payload).ToString();
                    //Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    //tRequest.ContentLength = byteArray.Length;
                    //using (Stream dataStream = tRequest.GetRequestStream())
                    //{
                    //    dataStream.Write(byteArray, 0, byteArray.Length);
                    //    using (WebResponse tResponse = tRequest.GetResponse())
                    //    {
                    //        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    //        {
                    //            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                    //                {
                    //                    String sResponseFromServer = tReader.ReadToEnd();
                    //                    //result.Response = sResponseFromServer;
                    //                }
                    //        }
                    //    } 
                    //}
                    var a = "OK";
                    return Ok(a);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        public bool CheckShipper_has_Order(string order_id_A, string order_id_B, double latitude, double longtitude)
        {

            var order_A = _context.Orders.Find(order_id_A);
            var customerAddress = _context.Addresses.Find(order_A.customer_address_id);
            var order_B = _context.Orders.Find(order_id_B);
            var sCoord = new GeoCoordinate(latitude, longtitude);

            var dis_cusA_to_market = sCoord.GetDistanceTo(new GeoCoordinate(customerAddress.lat ?? 0, customerAddress.@long ?? 0));
            var dis_market_to_customerB = order_B.dis_cus_to_market;

            var Sum_Distance = dis_cusA_to_market + dis_market_to_customerB;
            var _time = Sum_Distance / 30;
            _time = _time + 1;
            TimeSpan interval = TimeSpan.FromHours(_time ?? 0);
            var time_between = order_A.time_to_ship.Value.Subtract(order_B.time_to_ship.Value);
            if (interval.CompareTo(time_between) < 0)
            {
                return true;
            }

            //var order_has_shipper = _context.Orders.Where(x => x.id_shipper != null).Select(x => x.id_shipper).ToList();



            return false;
        }

    }
}
