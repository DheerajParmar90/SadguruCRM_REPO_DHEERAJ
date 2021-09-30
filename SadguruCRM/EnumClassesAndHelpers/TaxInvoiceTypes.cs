using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    public enum TaxInvoiceTypes : Byte
    {
        [Description("New Tax Invoice")]
        NewTaxInvoice = 1,
        [Description("Proforma Invoice")]
        ProformaInvoice = 2,
    }
}