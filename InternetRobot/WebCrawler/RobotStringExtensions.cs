using System;
using System.Linq;

namespace WebCrawler
{
    internal static class RobotStringExtensions
    {
        /// <summary> String extension for removing some specific characters from the string.
        /// </summary>
        internal static string NormalizeForUrl(this String input)
        {
            int i = 0;
            while (i<input.Length && 
                (input.ElementAt(input.Length - i - 1) == '/'
                || input.ElementAt(input.Length - i - 1) == '\''
                || input.ElementAt(input.Length - i - 1) == '\\'
                || input.ElementAt(input.Length - i - 1) == '"'
                || input.ElementAt(input.Length - i - 1) == ':'
                || input.ElementAt(input.Length - i - 1) == ';'))
            {
                i++;
            }
            if (i>0) return input.Remove(input.Length-i);
            return input;
        }

        /// <summary> String extension for replacing some specific characters from the string.
        /// </summary>
        internal static string EscapeUrl(this String input)
        {
            return input.Replace('/', '_').Replace(':', '_').Replace('?','_');
        }
    }
}
