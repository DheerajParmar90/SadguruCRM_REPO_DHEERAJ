using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    
        public enum GSTTypes : Byte
        {
            [Description("CGST/SGST")]
            V1= 1,
            [Description("IGST")]
            V2 = 2,
        }
    
}