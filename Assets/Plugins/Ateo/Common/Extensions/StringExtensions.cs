using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ateo.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }
        
        /// <summary>
        /// Parses a string into an enum value.
        /// </summary>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        
        /// <summary>
        /// Cuts the string at <paramref name="length"/>
        /// </summary>
        public static string Truncate(this string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= length ? value : value.Substring(0, length); 
        }

        /// <summary>
        /// Cuts the string at the first space starting at <paramref name="length"/>
        /// </summary>
        public static string TruncateAtWord(this string value, int length)
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length, StringComparison.Ordinal) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length, StringComparison.Ordinal));
        }
        
        public static string RemoveMultipleWhiteSpaces(this string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }
        
        public static string RemoveMultipleWhiteSpaces(this string input, char c)
        {
            return Regex.Replace(input, @"\s+", " ");
        }

        public static string RemoveLineBreaks(this string input)
        {
            return Regex.Replace(input, @"\r\n?|\n", string.Empty).Replace(Environment.NewLine, string.Empty);
        }
        
        public static string RemoveTags(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public static string ToTitleCase(this string text, string culture = "de-CH")
        {
            TextInfo cultInfo = new CultureInfo(culture, false).TextInfo;
            return cultInfo.ToTitleCase(text);
        }
        
        public static string ToLowerAddSpaces(this string text, bool preserveAcronyms = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            
            StringBuilder newText = new StringBuilder(text.Length * 2);

            newText.Append(char.ToLower(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append(' ');
                    }
                }

                newText.Append(char.ToLower(text[i]));
            }
            return newText.ToString();
        }
        
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}