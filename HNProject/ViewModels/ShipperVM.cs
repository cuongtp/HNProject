using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class ShipperVM
    {
        public string id { get; set; }

        public string username { get; set; }

        public double? @long { get; set; }

        public double? lat { get; set; }

        public double getDistance { get; set; }

        public string id_order { get; set; }

        public double? levels { get; set; }
        public string fcm_token { get; set; }
        public Nullable<int> isSelected { get; set; }
    }

   



}