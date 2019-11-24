using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.ViewModels
{
    public class FeedBackVM
    {
        public string id_feedback { get; set; }
        public string id_order { get; set; }
        public string id_role { get; set; }
        public Nullable<double> feedback_rate { get; set; }
        public string feedback_content { get; set; }
    }
}