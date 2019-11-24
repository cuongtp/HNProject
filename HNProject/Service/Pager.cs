using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public class Pager<T> where T : class
    {
        public int TotalPage { get; set; }

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }

        public IEnumerable<T> List{ get; set; }
    }
}