using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtension
    {
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static string Fa2En(this string str)
        {
            return str.Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")
                //iphone numeric
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

        public static string FixPersianChars(this string str)
        {
            return str.Replace("ﮎ", "ک")
                .Replace("ﮏ", "ک")
                .Replace("ﮐ", "ک")
                .Replace("ﮑ", "ک")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                .Replace(" ", " ")
                .Replace("‌", " ")
                .Replace("ھ", "ه"); //.Replace("ئ", "ی");
        }

        public static string CleanString(this string str)
        {
            return str?.Trim().FixPersianChars().Fa2En().NullIfEmpty();
        }

        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        public static string ToFormalPhoneNumber(this string input)
        {
            input = input?.CleanString();

            if (!IsPhoneNumber(input))
            {
                return string.Empty;
            }

            if (input != null && input[0] != '0')
            {
                if (input[0] == '+')
                {
                    input = input.TrimStart('+');
                }

                if (input[0] == '9' && input[1] == '8' && input[2] == '9') //input.StartsWith("989")
                {
                    int len = input.Length - 3;
                    input = "0" + input.Substring(2, len + 1);
                }

                if (input[0] != '0')
                {
                    input = "0" + input;
                }
            }

            return input;
        }
        
        public static string ToFormalEmail(this string input)
        {
            var formal = input.CleanString();

            if (string.IsNullOrWhiteSpace(formal) || !formal.IsValidEmail())
            {
                throw new InvalidDataException("Email is invalid");
            }

            return formal;
        }

        public static bool IsValidEmail(this string strIn)
        {
            if (string.IsNullOrWhiteSpace(strIn))
                return false;
            try
            {
                return Regex.IsMatch(strIn,
                    @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPhoneNumber(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) &&
                   Regex.IsMatch(input, @"(^09\d{9}$)|(^\+989\d{9}$)|(^9\d{9}$)|(^989\d{9}$)");
        }
    }
}