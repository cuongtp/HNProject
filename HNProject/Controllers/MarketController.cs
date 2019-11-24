using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;

namespace HNProject.Controllers
{
    [RoutePrefix("Markets")]
    public class MarketController : BaseController
    {
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_context.SMarkets.Select(x => new
            {
                x.id_market,
                x.name,
                address = x.GroupAddress.Addresses.Select(_ => new
                {
                    id_address = _.id_address,
                    address1 = _.address1,
                    description = _.description,
                    longtitute = _.@long,
                    latitute = _.lat,
                    priority = _.priority
                }),
                images = x.GroupImage.Images.Select(_ => _.url),
                brand = x.SMarketBrand.name,
            })) ;
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
                var market = _context.SMarkets.Where(x => x.id_market == id).FirstOrDefault();
                if (market == null)
                {
                    return NotFound();
                }
                return Ok(new
                {
                    id_market = market.id_market,
                    market_name = market.name,
                    address = market.GroupAddress.Addresses.Select(_ => new
                    {
                        address1 = _.address1,
                        description = _.description,
                        longtitute = _.@long,
                        latitute = _.lat,
                        priority = _.priority
                    }),
                    images = market.GroupImage.Images.Select(_ => _.url),
                    //brand = market.SMarketBrand.name ?? null,
                    //productInMarket = market.ProductInMarkets?.Select(_ => new
                    //{
                    //    name = _.Product.name,
                    //    price = _.Product.price,
                    //    qualify = _.Product.qualify
                    //}) ?? null
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
