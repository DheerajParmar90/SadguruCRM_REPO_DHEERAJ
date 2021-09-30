using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.ViewModels.SimplifiedModels
{
    public class ServiceModel
    {

        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceShortCode { get; set; }
        public int ServiceGroupID { get; set; }
    }
}