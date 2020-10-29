
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
namespace Manga.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        /// <summary>
        /// Returnst the Levenshtein Distance between 2 strings. This distance is used mostly for search purposes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int Levenshtein(this String source, String target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }

        /// <summary>
        /// Removes the extra spaces
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveExtaSpaces(this string text)
        {

            Regex regex = new Regex(@"\s{2,}", RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Singleline);
            text = regex.Replace(text.Trim(), " "); //This line removes extra spaces and make space exactly one.
            //To remove the  space between the end of a word and a punctuation mark used in the text we will
            //be using following line of code
            regex = new Regex(@"\s(\!|\.|\?|\;|\,|\:)"); // “\s” whill check for space near all puntuation marks in side ( \!|\.|\?|\;|\,|\:)”); )
            text = regex.Replace(text, "$1");
            return text;
        }

        /// <summary>
        /// Removes the illegal characters from the string which retains a file to be created with this name
        /// </summary>
        /// <param name="text">String to modify</param>
        /// <returns>Modified String which is valid for being a file name</returns>
        public static string RemoveIllegalCharacters(this string text)
        {
            string illegalCharacters = new string(System.IO.Path.GetInvalidPathChars()) + new string(System.IO.Path.GetInvalidFileNameChars());
            Regex regex = new Regex(string.Format("[{0}]", Regex.Escape(illegalCharacters)));
            text = regex.Replace(text, " ");
            return text;
        }
        /// <summary>
        /// Removes the illegal characters for sqlite database
        /// </summary>
        /// <param name="text">String to modify</param>
        /// <returns>Modified String which is valid for being an entry</returns>
        public static string SQLIllagalCharacters(this string text)
        {
            Regex regex = new Regex(string.Format("[{0}]", Regex.Escape("'")));
            text = regex.Replace(text, " ");
            return text;
        }

        public static bool Contains(this string source, string toCheck, System.StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool FindKeyByValue<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, TValue value, out TKey key)
        {
            if (dictionary == null)
                throw new System.ArgumentNullException("dictionary");

            foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value))
                {
                    key = pair.Key;
                    return true;
                }

            key = default(TKey);
            return false;
        }

        public static int IndexOf<T>(this LinkedList<T> list, T item)
        {
            var count = 0;
            for (var node = list.First; node != null; node = node.Next, count++)
            {
                if (item.Equals(node.Value))
                    return count;
            }
            return -1;
        }

        public static void ShiftLeft<T>(this T[] array, int start, int end)
        {
            T first = array[start];
            for (int i = start; i < end; i++)
                array[i] = array[i + 1];
            array[end] = first;
        }

        public static void ShiftRight<T>(this T[] array, int start, int end)
        {
            T last = array[end];
            for (int i = start + 1; i < end; i++)
                array[i] = array[i - 1];
            array[start] = last;
        }

        public static DateTime ToDateTime(this string input)
        {
            // just now
            if (input.Equals("now", StringComparison.InvariantCultureIgnoreCase) ||
                input.Equals("today", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Today;
            }
            else if (input.Equals("yesterday", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Today.AddDays(-1);
            }
            else if (Regex.IsMatch(input, "^.*( ago)$"))
            {
                String[] tokens = input.Split(' ');

                switch (tokens[1])
                {
                    case "second":
                    case "seconds":
                        return DateTime.Now.AddSeconds(-int.Parse(tokens[0]));
                    case "minute":
                    case "minutes":
                        return DateTime.Now.AddMinutes(-int.Parse(tokens[0]));
                    case "hour":
                    case "hours":
                        return DateTime.Now.AddHours(-int.Parse(tokens[0]));
                    case "day":
                    case "days":
                        return DateTime.Today.AddDays(-int.Parse(tokens[0]));
                    default:
                        break;
                }
            }

            return DateTime.Parse(input, CultureInfo.GetCultureInfo("en-us").DateTimeFormat);
        }
    }
}
