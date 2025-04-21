using System.Text.RegularExpressions;
using System.Web;

namespace Kromi.Domain.Extensions
{
    public static class StringExtension
    {
        public static string RemoveSpecialCharacters(this string input)
        {
            return Regex.Replace(input, @"\r\n?|\n|\t", string.Empty).Trim();
        }

        public static string ToLetterAndNumbers(this string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9]", string.Empty);
        }

        public static string ToFileName(this string input)
        {
            input = input.Replace(" ", "_");
            return Regex.Replace(input, "[^a-zA-Z0-9_]", string.Empty).ToLower();
        }

        public static string ToNumberEsp(this string input)
        {
            return input.Replace(".", string.Empty).Replace(",", ".");
        }

        public static string ToReferencia(this string input)
        {
            input = input.Trim();
            if (input.Length > 6)
                return input.Substring(input.Length - 6);
            else return input;
        }

        public static string ToShortReferencia(this string input)
        {
            input = input.Trim();
            if (input.Length > 5)
                return input.Substring(input.Length - 5);
            else return input;
        }

        public static string ToFileNameStrict(this string input)
        {
            input = input.Replace(" ", "_");
            return Regex.Replace(input, "[^a-zA-Z0-9_]", "").ToLower();
        }

        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");
            return properties != null ? string.Join("&", properties.ToArray()) : string.Empty;
        }
    }
}
