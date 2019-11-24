using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class CalculateVM
    {
        public string id_address { get; set; }
        public string customer_address_id { get; set; }
        public Nullable<System.TimeSpan> time_to_ship { get; set; }
        public virtual ICollection<OrderDetailCalculateVM> OrderDetails { get; set; }


        public class OrderDetailCalculateVM
        {
            public string id_orderdetail { get; set; }
            public string id_order { get; set; }
            public string id_product { get; set; }
            public string id_market { get; set; }
            public Nullable<double> price { get; set; }
            public Nullable<double> quanlity { get; set; }
            public Nullable<int> priority { get; set; }
        }
    }
}