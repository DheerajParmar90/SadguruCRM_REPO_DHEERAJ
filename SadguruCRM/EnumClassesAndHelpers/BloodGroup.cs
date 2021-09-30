using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    public enum  BloodGroup
    {
        [Description("A+")]
        APositive = 1,
        [Description("A-")]
        ANegative = 2,
        [Description("B+")]
        BPositive = 3,
        [Description("B-")]
        BNegative = 4,
        [Description("O+")]
        OPositive = 5,
        [Description("O-")]
        ONegative = 6,
        [Description("AB+")]
        ABPositive = 7,
        [Description("AB-")]
        ABNegative = 8


    }
}