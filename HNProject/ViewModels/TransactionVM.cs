using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class TransactionVM
    {
        public string id_transaction { get; set; }
        public string id_order { get; set; }
        public double money { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }

    }
}