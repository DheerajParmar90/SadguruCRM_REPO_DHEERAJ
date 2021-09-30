using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.ViewModels.SimplifiedModels
{
    public class LocationModel
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int BranchID { get; set; }
    }
}