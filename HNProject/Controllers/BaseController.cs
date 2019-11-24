using HNProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HNProject.Controllers
{
    public class BaseController : ApiController
    {
        protected GoStoreDbContext _context;

        public BaseController()
        {
            _context = new GoStoreDbContext();
        }
    }
}
