using SadguruCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.ViewModels
{
    public class ContractServicesViewModel
    {
        public int ContractID { get; set; }
        public List<Contract_Services_Mapping> contractServices { get; set; }
    }
}