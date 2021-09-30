using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SadguruCRM.EnumClassesAndHelpers
{
    public enum Gender
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2
        //    ,
        //[Description("Prefer Not To Mention")]
        //PreferNotToMention = 3
    }
}