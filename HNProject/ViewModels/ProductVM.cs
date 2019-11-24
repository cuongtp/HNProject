using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class ProductVM
    {
        public string id_product { get; set; }
        public string name { get; set; }
        public string id_group_image { get; set; }
        public Nullable<double> price { get; set; }
        public string qualify { get; set; }
        public string description { get; set; }
        public Nullable<int> type { get; set; }

        public ICollection<GroupImageVM> GroupImages { get; set; }
        public ICollection<Brand> Brands { get; set; }
        public ICollection<ProductCategoryVM> ProductCategories { get; set; }

        public ICollection<ProductInMarket> ProductInMarkets { get; set; }
    }

    

    public class GroupImageVM
    {
        public string id_group { get; set; }
        public virtual ICollection<ImageVM> Images { get; set; }
    }

    public class ImageVM
    {
        public string id_image { get; set; }
        public string id_group { get; set; }
        public string url { get; set; }
    }

    public class Brand
    {
        public string id_brand { get; set; }
        public string name { get; set; }
    }

    public class ProductCategoryVM
    {
        public string id_cate { get; set; }
        public string id_product { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    }

    public class ProductInMarket
    {
        public string id_product { get; set; }
        public string id_market { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
    }
}