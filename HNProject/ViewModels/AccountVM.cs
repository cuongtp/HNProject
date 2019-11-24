using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class AccountVM
    {
        public string id_account { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public string email { get; set; }
        public string fb_id { get; set; }
        public string id_group_address { get; set; }
        public string id_group_image { get; set; }
        public string id_role { get; set; }
        public Nullable<int> state { get; set; }

        public string fcm_token { get; set; }
        public string connection_id { get; set; }
        public int isSelected { get; set; }
        //public ICollection<FavouriteProduct> FavouriteProducts { get; set; }

        public ICollection<LevelVM> Levels { get; set; }

        
    }
    //public class FavouriteProduct
    //{
    //    public string id_product { get; set; }
    //    public string id_customer { get; set; }
    //    public Nullable<System.DateTime> created_date { get; set; }
    //}

    public class AddressViewModels
    {
        public string Address { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        public string Description { get; set; }
    }

    public class CMMAddressVM
    {
        public string AccountId { get; set; }

        public ICollection<AddressViewModels> Addresses { get; set; }
    }
}