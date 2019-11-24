using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public class PaginationImp : IPagination
    {
        public Pager<T> ToPagedList<T>(int pageIndex, int pageSize, IEnumerable<T> list) where T : class
        {
            int totalPage = (int)Math.Ceiling((double)list.Count() / pageSize);
            int Left = pageIndex - 1;
            int Right = pageIndex + 1;
            if (Left <= 0)
            {
                Right -= (Left - 1);
                Right = Math.Min(Right, totalPage);
                Left = 1;
            }
            if (Right > totalPage)
            {
                Right = totalPage;
                Left = Math.Max(Right - 2, 1);
                Left = Math.Max(1, Left);
            }

            return new Pager<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Left = Left,
                TotalPage = totalPage,
                TotalItems = list.Count(),
                Right = Right,
                List = list.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };
        }
    }
}