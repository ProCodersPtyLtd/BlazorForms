using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public static class DateConvertHelper
    {
        public static string DateValueText(DateTime? dateTime)
        {
            var str = dateTime == null ? "" : dateTime.Value.ToString();
            return str;
        }

        public static string DateValueShortText(DateTime? dateTime)
        {
            var str = dateTime == null ? "" : dateTime.Value.ToShortDateString();
            return str;
        }
    }
}
