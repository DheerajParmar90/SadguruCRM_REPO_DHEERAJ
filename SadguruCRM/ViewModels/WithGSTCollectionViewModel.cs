using SadguruCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.ViewModels
{
    public class WithGSTCollectionViewModel
    {
        public Invoice singleInvoice { get; set; }
        public List<Collection_Entry> listCollectionEntries { get; set; }
    }
}