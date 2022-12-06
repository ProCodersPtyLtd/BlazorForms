using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace BlazorForms.Forms
{
    // ToDo: rename this class to more appropriate
    public static class JsonPathExtensions
    {
        public static string ReplaceLambdaVar(this string selectorString)
        {
            var result = selectorString.Trim();
            var values = result.Split('.');

            // only when 2 or more parts, plane value like 'g' returned as string.Empty
            if (values.Length == 1)
            {
                return string.Empty;
            }
            else if (values.Length > 1)
            {
                result = result.Substring(values[0].Length + 1);
            }

            result = "$." + result;
            return result;
        }

        public static string ToSql(this object source)
        {
            if (source == null)
            {
                return "NULL";
            }

            string convertedValue;
            var dataTypeName = source.GetType().Name;

            if (dataTypeName == "Nullable`1")
            {
                dataTypeName = Nullable.GetUnderlyingType(source.GetType()).Name;
            }

            switch (dataTypeName)
            {
                case "DateTime":
                    convertedValue = $"\"{((DateTime)source).ToString("yyyy-MM-dd")}\"";
                    break;

                case "String":
                    convertedValue = $"\"{source}\"";
                    break;

                default:
                    convertedValue = $"{source}";
                    break;
            }

            return convertedValue;
        }

        public static void SetPropertyValue(this object target, string bindingProperty, object value)
        {
            var property = target.GetType().GetProperty(bindingProperty);
            object convertedValue = value;

            try
            {
                if (value == null || value.ToString().Trim() == "")
                {
                    convertedValue = null;
                }
                else
                {
                    var dataType = property.PropertyType;
                    var isNullable = Nullable.GetUnderlyingType(dataType) != null;

                    if (isNullable)
                    {
                        dataType = Nullable.GetUnderlyingType(dataType);
                    }

                    if (dataType.IsEnum)
                    {
                        if (isNullable && value.ToString() == "0")
                        {
                            property.SetValue(target, null);
                            return;
                        }

                        convertedValue = Enum.Parse(dataType, value.ToString());
                    }
                    else
                    {
                        var dataTypeName = dataType.Name;

                        switch (dataTypeName)
                        {
                            case "Int32":
                                convertedValue = Convert.ToInt32(value);
                                break;

                            case "Boolean":
                                convertedValue = Convert.ToBoolean(value);
                                break;

                            case "Decimal":
                                convertedValue = Convert.ToDecimal(value);
                                break;
                        }
                    }
                }

                property.SetValue(target, convertedValue);
            }
            catch (FormatException fexc)
            { 
            }
        }

        public static object GetPropertyValue(this object target, string bindingProperty)
        {
            if (bindingProperty == string.Empty)
            {
                return target;
            }

            var property = target.GetType().GetProperty(bindingProperty);
            var result = property.GetValue(target);
            return result;
        }

        public static bool DefinesComponentParameter(this Type component, string prop, Type attr)
        {
            var result = (component.GetProperties().Any(p => p.Name == prop) &&
                component.GetProperty(prop).GetCustomAttributes(attr).Any());

            return result;
        }
    }
}
