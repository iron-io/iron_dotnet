using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace IronIO.Core.Extensions
{
    public static class ExtensionsForString
    {
        private static Regex _tokenRegex;

        private static Regex TokenRegex
        {
            get { return _tokenRegex ?? (_tokenRegex = new Regex(@"(\{[A-Z0-9_]+(:.+?)?\})", RegexOptions.IgnoreCase | RegexOptions.Compiled)); }
        }

        /// <summary>
        /// Returns a <paramref name="comparison"> </paramref> specific version of the contains method.
        /// </summary>
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }
            if (String.IsNullOrEmpty(value))
            {
                return true;
            }
            return s.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Executes a String.Format() call on a string with named token parameters like "{lastName}, {firstName}" or "{dueDate:MM-dd-yyy}"
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="args"> if the string contains tokens like {firstName} and {lastName}, then you should pass a dictionary with keys that match those names. </param>
        /// <param name="valueEncoder"> A delegate method such as like: AddressOf HttpUtility.UrlEncode </param>
        /// <returns> </returns>
        public static string FormatWithNamedArgs(this string format, IDictionary<string, object> args, Func<string, string> valueEncoder)
        {
            MatchCollection matches = TokenRegex.Matches(format);

            for (int i = 0; i < matches.Count; i++)
            {
                string name = matches[i].Value;

                string formatString = "{0}";
                string key = name.Trim(new[] {'{', '}'});

                if (key.Contains(":"))
                {
                    string[] parts = key.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        key = parts[0];
                        formatString = string.Concat("{0:", parts[1], "}");
                    }
                }

                if (args.ContainsKey(key))
                {
                    string value = string.Format(formatString, args[key]);
                    if (valueEncoder != null)
                    {
                        value = valueEncoder.Invoke(value);
                    }
                    format = format.Replace(name, value);
                }
            }

            return format;
        }

        /// <summary>
        /// Executes a String.Format() call on a string with named token parameters like "{lastName}, {firstName}" or "{dueDate:MM-dd-yyy}"
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="args">
        /// if the string contains tokens like {firstName} and {lastName}, then pass an object with matching property names or use the object initializer syntax - New With
        /// {.firstName = "Joe", .lastName = "Smith"}
        /// </param>
        /// <returns> </returns>
        public static string FormatWithNamedArgs(this string format, object args)
        {
            return FormatWithNamedArgs(format, args, null);
        }

        /// <summary>
        /// Executes a String.Format() call on a string with named token parameters like "{lastName}, {firstName}" or "{dueDate:MM-dd-yyy}"
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="args">
        /// if the string contains tokens like {firstName} and {lastName}, then pass an object with matching property names or use the object initializer syntax - New With
        /// {.firstName = "Joe", .lastName = "Smith"}
        /// </param>
        /// <param name="valueEncoder"> A delegate method such as like: AddressOf HttpUtility.UrlEncode </param>
        /// <returns> </returns>
        public static string FormatWithNamedArgs(this string format, object args, Func<string, string> valueEncoder)
        {
            var argDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (args != null)
            {
                argDictionary.AddValues(args);
            }

            return FormatWithNamedArgs(format, argDictionary, valueEncoder);
        }

        /// <summary>
        /// Executes a String.Format() call on a string with named token parameters like "{lastName}, {firstName}" or "{dueDate:MM-dd-yyy}"
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="args"> if the string contains tokens like {firstName} and {lastName}, then you should pass a dictionary with keys that match those names. </param>
        /// <returns> </returns>
        public static string FormatWithNamedArgs(this string format, IDictionary<string, object> args)
        {
            return FormatWithNamedArgs(format, args, null);
        }

        /// <summary>
        /// Returns the parsed date time value or the default value if the conversion failed.
        /// </summary>
        /// <param name="s"> The s. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <param name="exactPattern"> The pattern to match such as "yyyyMMdd" </param>
        /// <returns> </returns>
        public static DateTime ToDateTime(this string s, DateTime defaultValue, string exactPattern)
        {
            DateTime returnValue = defaultValue;

            DateTime outValue;

            if (exactPattern != null)
            {
                if (DateTime.TryParseExact(s, exactPattern, null, DateTimeStyles.None, out outValue))
                {
                    returnValue = outValue;
                }
            }
            else
            {
                if (DateTime.TryParse(s, out outValue))
                {
                    returnValue = outValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the parsed date time value or the default value if the conversion failed.
        /// </summary>
        public static DateTime ToDateTime(this string s, DateTime defaultValue)
        {
            return ToDateTime(s, defaultValue, Convert.ToString(null));
        }

        /// <summary>
        /// Returns the parsed date time value or the default value if the conversion failed.
        /// </summary>
        /// <param name="s"> The s. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <param name="exactPattern"> A collection of patterns to match such as {"yyyy.MM.dd", "yyyy-MM-dd"} </param>
        /// <returns> </returns>
        public static DateTime ToDateTime(this string s, DateTime defaultValue, IEnumerable<string> exactPattern)
        {
            DateTime returnValue = defaultValue;

            DateTime outValue;

            string[] patterns = null;

            if (exactPattern != null)
            {
                patterns = exactPattern.ToArray();
            }

            if (patterns != null && patterns.Any())
            {
                if (DateTime.TryParseExact(s, patterns, null, DateTimeStyles.None, out outValue))
                {
                    returnValue = outValue;
                }
            }
            else
            {
                if (DateTime.TryParse(s, out outValue))
                {
                    returnValue = outValue;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the parsed date time value or null if the conversion failed.
        /// </summary>
        public static DateTime? ToDateTime(this string s)
        {
            DateTime value = ToDateTime(s, DateTime.MinValue, Convert.ToString(null));
            if (value == DateTime.MinValue)
            {
                return null;
            }
            return value;
        }

        public static string ToString<T>(this T? nullableValue, string format, IFormatProvider formatProvider = null, string nullDisplayValue = "") where T : struct, IFormattable
        {
            return nullableValue.HasValue ? nullableValue.Value.ToString(format, formatProvider) : nullDisplayValue;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We want to make this user friendly and return the default value on all failures")]
        public static TValue As<TValue>(this string value, TValue defaultValue = default(TValue))
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof (TValue));
                if (converter.CanConvertFrom(typeof (string)))
                {
                    return (TValue) converter.ConvertFrom(value);
                }
                // try the other direction
                converter = TypeDescriptor.GetConverter(typeof (string));
                if (converter.CanConvertTo(typeof (TValue)))
                {
                    return (TValue) converter.ConvertTo(value, typeof (TValue));
                }
            }
                // ReSharper disable EmptyGeneralCatchClause
            catch
                // ReSharper restore EmptyGeneralCatchClause
            {
                // eat all exceptions and return the defaultValue, assumption is that its always a parse/format exception
            }
            return defaultValue;
        }
    }
}