using System.Text.RegularExpressions;

namespace Kromi.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string input)
        {
            // Use a regular expression to replace non-alphanumeric characters with an empty string
            return Regex.Replace(input, "[^a-zA-Z0-9]", "");
        }

        public static string ToFileName(this string input)
        {
            input = input.Replace(" ", "_");
            return Regex.Replace(input, "[^a-zA-Z0-9_.]", "").ToLower();
        }
        public static string ToFileNameStrict(this string input)
        {
            input = input.Replace(" ", "_");
            return Regex.Replace(input, "[^a-zA-Z0-9_]", "").ToLower();
        }
    }
}
