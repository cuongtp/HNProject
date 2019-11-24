using HNProject.Models;
using HNProject.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web;
using HNProject.Service;
using Microsoft.AspNet.SignalR.Client;
using System.Web.Script.Serialization;

namespace HNProject.Controllers
{
    [RoutePrefix("TestPayment")]
    public class ShipperTestPaymentController : BaseController
    {
        [HttpGet, Route("")]
        public async System.Threading.Tasks.Task<IHttpActionResult> GetAsync(string created_at, string id, string mrc_order_id,string stat,string total_amount,string txn_id,string updated_at,string checksum)
        {
            Redirect("smarket://market/");
            //if(stat=="p")
            //{
            //    return Redirect("smarket://market");
            //}
            //Customer from order
            var customer = _context.Orders.Select(x => x.Account);

            ////Shipper from order
            //var shipper = _context.Orders.Select(x => x.Account1);

            //order từ order_code
            var order_fcm = _context.Orders.Find(mrc_order_id);

            var __order = _context.Orders.Find(mrc_order_id);
            double latitude = _context.Addresses.Find(__order.id_address).lat ?? 0;
            double longtitude = _context.Addresses.Find(__order.id_address).@long ?? 0;
            var order_total = __order.total_amount ?? 0;
            try
            {
                Redirect("smarket://market/");
                JWTManagement checkOut = new JWTManagement();
                bool rs = checkOut.verifyPaymentUrl(id,mrc_order_id,txn_id,total_amount,stat,created_at,updated_at,checksum);
                byte[] buffer = Guid.NewGuid().ToByteArray();
                var trans_id = BitConverter.ToInt64(buffer, 0).ToString();
                //rs = true;
                //if(order_total != double.Parse(total_amount))
                //{
                //    rs = false;
                //    //Notify Customer
                //    Console.WriteLine("false");
                //    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                //    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                //    string msg = "Thanh toán thất bại!";
                //    int count = 1;
                //    Account _customer = _context.Accounts.Find(__order.id_customer);
                //    var connectionID_cus = _customer.connection_id;
                //    await hubConnection.Start();

                //    var change = hubConnection.State;
                //    var connection_id = hubConnection.ConnectionId;
                //    await hubProxy.Invoke("NotifyCustomer", connectionID_cus, msg);

                //    return Redirect("smarket://market");

                //}
                //rs = true;
                if(rs == true)
                {
                    //HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    //IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");
                    //var json_object = new
                    //{
                    //    msg = "Thanh toán thành công!",
                    //    id_order = mrc_order_id,
                    //};

                    //JavaScriptSerializer serializer = new JavaScriptSerializer();
                    //var Json_str  = serializer.Serialize(json_object);


                    //string msg = Json_str;
                    //Account _customer = _context.Accounts.Find(__order.id_customer);
                    //var connectionID_cus = _customer.connection_id;
                    //await hubConnection.Start();

                    //var change = hubConnection.State;
                    //var connection_id = hubConnection.ConnectionId;
                    
                    //await hubProxy.Invoke("NotifyCustomer", connectionID_cus, msg);

                    Redirect("smarket://market/" + mrc_order_id);
                }

                if (rs == false)
                {

                    Console.WriteLine("false");
                    HubConnection hubConnection = new HubConnection("http://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");

                    var json_object = new
                    {
                        msg = "Thanh toán thất bại!",
                        id_order = mrc_order_id,
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

                   return Redirect("smarket://market/" + mrc_order_id);
                }


                __order.state = 1;
                _context.Transactions.Add(new Transaction
                {
                    id_transaction = trans_id,
                    id_order = mrc_order_id,
                    money = double.Parse(total_amount),
                    created_time = DateTime.Now
                }) ;
                _context.SaveChanges();
                


                //var order = _context.Orders.Find(id_order);
                var sCoord = new GeoCoordinate(latitude, longtitude);
                List<Order> list_order_true = new List<Order>();
                
                var order_has_shipper = _context.Orders.Where(x => x.id_shipper != null).ToList();
                foreach (var item in order_has_shipper)
                {
                    var id_or = item.id_order;
                    if (CheckShipper_has_Order(id_or, mrc_order_id, latitude, longtitude) == true)
                    {
                        list_order_true.Add(item);
                    }
                }

                var list_id_shipper_in_order = order_has_shipper.Select(x => x.id_shipper).ToList();

                var _shipper = new List<ShipperVM>();
                foreach (var item in list_order_true)
                {
                    _shipper = _context.Accounts.AsEnumerable().Where(x => x.id_account == item.id_shipper && x.isSelected ==0 && x.connection_id != "0" && x.state == 0).Select(x => new ShipperVM

                    {
                        id = x.id_account,
                        username = x.username,
                        lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                        @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                        levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                        fcm_token = x.fcm_token,
                        isSelected = x.isSelected?? 0,
                        getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0, x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0))
                    }).ToList();
                }

                var list_shipper_has_order = _shipper.GroupBy(x => x.id).Select(y => y.First());
                var list_shipper_no_order = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.isSelected == 0 && x.connection_id != "0" && x.state == 0).Select(x => new ShipperVM 
                {
                    id = x.id_account,
                    username = x.username,
                    lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                    @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                    levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                    fcm_token = x.fcm_token,
                    id_order = mrc_order_id,
                    isSelected = x.isSelected?? 0,
                    getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0, x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0))
                }).ToList() ?? null;
                //if (_shipper.Count > 0)
                //{
                //    foreach (var item in list_shipper_no_order.AsEnumerable())
                //    {
                //        foreach (var item1 in _shipper)
                //        {
                //            if (item.id == item1.id && list_id_shipper_in_order.Count >0)
                //            {
                //                list_shipper_no_order.Remove(item);
                //            }
                //        }
                //    }
                //}



                //var address = _context.Accounts.AsEnumerable().Where(x => x.id_role == "1" && x.isSelected != 0).Select(x => new ShipperVM
                //{
                //    id = x.id_account,
                //    username = x.username,
                //    lat = x.GroupAddress.Addresses.FirstOrDefault().lat,
                //    @long = x.GroupAddress.Addresses.FirstOrDefault().@long,
                //    levels = x.Levels.Select(xx => xx.point).FirstOrDefault(),
                //    fcm_token = x.fcm_token,
                //    id_order = mrc_order_id,
                //    getDistance = sCoord.GetDistanceTo(new GeoCoordinate(x.GroupAddress.Addresses.FirstOrDefault().lat ?? 0, x.GroupAddress.Addresses.FirstOrDefault().@long ?? 0))
                //}).ToList().OrderBy(x => x.getDistance);

                var list = new List<ShipperVM>();
                var list1 = new List<ShipperVM>();
                var list2 = new List<ShipperVM>();


                foreach (var item in list_shipper_no_order)
                {
                    if (item.getDistance <= 10000)
                    {
                        list.Add(item);
                    }
                }

                foreach (var item in list_shipper_has_order)
                {
                    if (!list.Any(s => s.id == item.id))
                    {
                        list.Add(item);
                    }
                }


                foreach (var item in list.OrderBy(x => x.levels).Reverse().Take(5))
                {
                    list1.Add(item);
                }
                //if(id_shipper != null)
                //{
                //    var temp = list1.FirstOrDefault(x => x.id == id_shipper);
                //    if (temp != null)
                //    {
                //        list1.Remove(temp);
                //    }
                //}


                if (list1.Count == 0)
                {
                    //Redirect("smarket://market");

                    Console.WriteLine("false");
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");

                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");


                    var json_object = new
                    {
                        msg = "Không tìm thấy shipper",
                        id_order = mrc_order_id,
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

                   return Redirect("smarket://market/" + mrc_order_id);
                }
                else
                {
                    var accoutShipper = _context.Accounts.Find(list1.First().id);
                    accoutShipper.isSelected = 1;
                    _context.SaveChanges();
                    //list1.Reverse();
                    HubConnection hubConnection = new HubConnection("https://localhost:44333");
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("NotifyHub");

                    string msg = "Bạn có một đơn hàng!";
                    int count = 1;
                    ShipperVM shipper_ = list1.First();
                    var connectionID_shipper = _context.Accounts.Where(x => x.id_role == "1" && x.id_account == shipper_.id).Select(x => x.connection_id).FirstOrDefault();
                    await hubConnection.Start();

                    var change = hubConnection.State;
                    var connection_id = hubConnection.ConnectionId;
                    //while (hubConnection.ConnectionId == "" || hubConnection.ConnectionId == null) { }
                    //var conn_id = hubProxy.Invoke("getConnectionId");
                    await hubProxy.Invoke("NotifyShipper", connectionID_shipper, mrc_order_id, count, msg, shipper_.id, latitude, longtitude);

                    return Redirect("smarket://market/" +mrc_order_id);
                    //return Ok(list1);

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

                }

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
            var _time = Sum_Distance / 30000;
            //30km/h = 30000m/h
            _time = _time + 1;
            TimeSpan interval = TimeSpan.FromHours(_time ?? 0);
            var time_between =  order_A.time_to_ship.Value.Subtract(order_B.time_to_ship.Value);
          
            if (interval.CompareTo(time_between) < 0)
            {
                return true;
            }

            //var order_has_shipper = _context.Orders.Where(x => x.id_shipper != null).Select(x => x.id_shipper).ToList();



            return false;
        }

    }
}
