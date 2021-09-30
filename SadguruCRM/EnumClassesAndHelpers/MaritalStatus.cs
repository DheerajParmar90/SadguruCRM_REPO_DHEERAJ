using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    public enum MaritalStatus
    {
        [Description("Unmarried")]
        Unmarried = 1,
        [Description("Married")]
        Married = 2,
        [Description("Divorced")]
        Divorced = 3
    }
}