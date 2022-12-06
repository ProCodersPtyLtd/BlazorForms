using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Shared
{
    public static class ObjectConvert
    {
        public static string AsString(this object src)
        {
            return src == null ? (string)null : Convert.ToString(src);
        }

        public static int? AsInt(this object src)
        {
            return src == null ? (int?)null : Convert.ToInt32(src);
        }
        public static decimal? AsDecimal(this object src)
        {
            return src == null ? (decimal?)null : Convert.ToDecimal(src);
        }

        public static DateTime? AsDate(this object src)
        {
            return src == null ? (DateTime?)null : Convert.ToDateTime(src);
        }

        public static bool? AsBool(this object src)
        {
            return src == null ? (bool?)null : Convert.ToBoolean(src);
        }

        public static Money AsMoney(this object src)
        {
            return (Money)(src);
        }
    }
}
