using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class AddressVM
    {
        public string id_address { get; set; }
        public Nullable<double> @long { get; set; }
        public Nullable<double> lat { get; set; }
        public string address1 { get; set; }
        public string id_group_address { get; set; }
        public string description { get; set; }
        public Nullable<int> priority { get; set; }
    }
}