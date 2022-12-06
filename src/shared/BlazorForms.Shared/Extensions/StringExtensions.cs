using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorForms.Shared
{
    public static class StringExtensions
    {
        public static string LastWord(this string source, char separator = '.')
        {
            var split = source.Split(separator);
            return split[split.Length - 1];
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string ReplaceEnd(this string source, string end, string newEnd)
        {
            var result = Regex.Replace(source, $"{end}$", newEnd);
            return result;
        }

        public static string RemoveQuotesNull(this string source)
        {
            var result = RemoveQuotes(source);
            result = result.Trim() == "" ? null : result;
            return result;
        }

        public static string RemoveQuotes(this string source)
        {
            if (source != null && source.Count() > 0 && (source.First() == '\'' || source.First() == '"'))
            {
                source = source.Remove(0, 1);
            }

            if (source != null && source.Count() > 0 && (source.Last() == '\'' || source.Last() == '"'))
            {
                source = source.Remove(source.Length-1, 1);
            }

            return source;
        }

        public static string SplitWords(this string source)
        {
            var sb = new StringBuilder();

            foreach(var c in source)
            {
                if (char.IsUpper(c))
                {
                    sb.Append(" ");
                }

                sb.Append(c);
            }

            return sb.ToString().Trim();
        }
    }
}
