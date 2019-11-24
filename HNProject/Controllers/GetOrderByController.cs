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

namespace HNProject.Controllers
{
    [RoutePrefix("GetOrderBy")]
    public class GetOrderByController : BaseController
    {

        [HttpPost, Route("Check_and_Create_Trans")]
        public async System.Threading.Tasks.Task<IHttpActionResult> PostAsync(string id_order)
        {
            try
            {
                var order = _context.Orders.Find(id_order);
                if(order == null)
                {
                    return NotFound();
                }
                JWTManagement jwtObject = new JWTManagement();
                var token = jwtObject.getToken();
                string uri = "https://sandbox-api.baokim.vn/payment/api/v4/order/detail?mrc_order_id="+id_order+"&jwt="+token;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                HttpContent content = response.Content;
                string data = await content.ReadAsStringAsync();
                dynamic d = JObject.Parse(data);
                var stat = d.stat;

                bool rs = false;
                if (stat == "c")
                {
                    rs = true;
                    var trans = _context.Transactions.Where(x => x.id_order == id_order).ToList();
                    if(trans.Count != 0)
                    {
                        return Ok(trans);
                    }

                    byte[] buffer = Guid.NewGuid().ToByteArray();
                    var trans_id = BitConverter.ToInt64(buffer, 0).ToString();
                    if (trans.Count == 0)
                    {
                        _context.Transactions.Add(new Transaction
                        {
                            id_transaction = trans_id,
                            id_order = id_order,
                            money = order.total_amount,
                            created_time = DateTime.Now,
                        });
                        _context.SaveChanges();
                        return Ok(trans);
                    }
                }


                if (rs == false)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet, Route("GetOrderByCustomer_id")]
        public IHttpActionResult GetById(string id_customer, [FromUri]int pageIndex = 1, [FromUri]int pageSize = 100)
        {
            var customer = _context.Accounts.FirstOrDefault(x => x.id_account == id_customer);
            var paginationImp = new PaginationImp();
            try
            {
                if (customer == null)
                {
                    return NotFound();
                }
                var orders = _context.Orders.Where(x => x.id_customer == customer.id_account).ToList();
                if (orders == null)
                {
                    return NotFound();
                }

                

                var model = orders.Select(xx => new
                {
                    id_order = xx.id_order,
                    created_date = xx.created_date,
                    id_customer = xx.id_customer,
                    id_shipper = xx.id_shipper,
                    state = xx.state,
                    id_group_image = xx.id_group_image,
                    id_address = xx.id_address,
                    point = xx.point,
                    shipping_cost = xx.shipping_cost,
                    time_to_ship = xx.time_to_ship,
                    customer_address_id = xx.customer_address_id,
                    dis_cus_to_market = xx.dis_cus_to_market,
                    phone = xx.phone,
                    name = xx.name,
                    taking_time = xx.taking_time,
                    done_time = xx.done_time,
                    order_code = xx.order_code,
                    customer_comment = xx.customer_comment,
                    system_cost = xx.system_cost,
                    total_amount = xx.total_amount,
                    token = xx.token,
                    markert_id = xx.OrderDetails.Select(x => x.id_market),
                    OrderDetails = xx.OrderDetails.Select(__ => new
                    {
                        id_orderdetail = __.id_orderdetail,
                        id_order = __.id_order,
                        product  = new 
                        { 
                            id_product = __.Product.id_product,
                            name = __.Product.name,
                            groupImages = __.Product.GroupImage.Images.Select(_ => _.url),
                            price = __.Product.price,
                            qualify = __.Product.qualify,
                            description = __.Product.description,
                            type = __.Product.type,
                            brandName = __.Product.Brands.Select(_ => _.name),
                            productCategory = __.Product.ProductCategories.Select(_ => _.Category.name)
                        },
                        id_market = __.id_market,
                        price = __.price,
                        quanlity = __.quanlity,
                        priority = __.priority,

                    }).ToList()
                }).ToList().OrderByDescending(x => x.created_date);

                return Ok(paginationImp.ToPagedList(pageIndex, pageSize, model));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("GetOrderByShipper_id")]
        public IHttpActionResult GetByShipperId(string id_shipper)
        {
            var Shipper = _context.Accounts.FirstOrDefault(x => x.id_account == id_shipper);
            var paginationImp = new PaginationImp();
            try
            {
                if (Shipper == null)
                {
                    return NotFound();
                }
                var orders = _context.Orders.Where(x => x.id_shipper == Shipper.id_account).ToList();
                if (orders == null)
                {
                    return NotFound();
                }



                var model = orders.Select(xx => new
                {
                    id_order = xx.id_order,
                    shipper_id = xx.id_shipper,
                    customer_id = xx.id_customer,
                    order_state = xx.state,
                    point = xx.point,
                    shipping_cost = xx.shipping_cost,
                    time_to_ship = xx.time_to_ship,
                    customer_address_id = xx.customer_address_id,
                    dis_cus_to_market = xx.dis_cus_to_market,
                    //shipper_name = shipper.Select(x => x.username),
                    //customer_name = customer.Select(x => x.username),
                    created_date = xx.created_date,
                    id_address = xx.id_address,
                    phone = xx.phone,
                    name = xx.name,
                    taking_time = xx.taking_time,
                    done_time = xx.done_time,
                    order_code = xx.order_code,
                    customer_comment = xx.customer_comment,
                    system_cost = xx.system_cost,
                    total_amount = xx.total_amount,
                    token = xx.token,
                    orderDetails = xx.OrderDetails.Select(__ => new
                    {
                        __.Product.name,
                        Price = __.Product.price * __.quanlity,
                        __.Product.qualify,
                        market_name = __.SMarket.name,

                    }).ToList()
                }).ToList().OrderByDescending(x => x.done_time);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet, Route("GetOrderByOrder_id")]
        public IHttpActionResult GetByOrderID(string id_order)
        {
            var order = _context.Orders.Find(id_order);
            var paginationImp = new PaginationImp();
            try
            {
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
                    markert_id = order.OrderDetails.Select(x => x.id_market),
                    OrderDetails = order.OrderDetails.Select(__ => new
                    {
                        id_orderdetail = __.id_orderdetail,
                        id_order = __.id_order,
                        product = new
                        {
                            id_product = __.Product.id_product,
                            name = __.Product.name,
                            groupImages = __.Product.GroupImage.Images.Select(_ => _.url),
                            price = __.Product.price,
                            qualify = __.Product.qualify,
                            description = __.Product.description,
                            type = __.Product.type,
                            brandName = __.Product.Brands.Select(_ => _.name),
                            productCategory = __.Product.ProductCategories.Select(_ => _.Category.name)
                        },
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

        [HttpGet, Route("GetTransactionByOrderID")]
        public IHttpActionResult GetTransactionByOrderID(string order_id)
        {
            var transactions = _context.Transactions.Where(x => x.id_order == order_id).ToList();

            if(transactions == null)
            {
                return NotFound();

            }

            var trans = transactions.Select(x => new
            {
                id_transaction = x.id_transaction,
                id_order = x.id_order,
                total_amount = x.money,
                created_time = x.created_time
            }).ToList();
            return Ok(trans);
        }


    }
}
