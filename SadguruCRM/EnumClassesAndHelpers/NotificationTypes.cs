using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    public enum NotificationTypes
    {
        [Description("Lead Created")]
        NewLead = 1,
        [Description("NewEstimate")]
        NewEstimate = 2,
        [Description("NewContract")]
        NewContract = 3
    }
}