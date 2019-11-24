using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public interface IPagination
    {
        Pager<T> ToPagedList<T>(int pageIndex, int pageSize, IEnumerable<T> list) where T : class;

    }
}