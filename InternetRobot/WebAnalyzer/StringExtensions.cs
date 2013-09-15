using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAnalyzer
{
    public static class StringExtensions
    {
        public static string ReplacePolishCharacters(this string html)
        {
            string ret = html;
            ret = ret.Replace('ą', 'a');
            ret = ret.Replace('ć', 'c');
            ret = ret.Replace('ź', 'z');
            ret = ret.Replace('ż', 'z');
            ret = ret.Replace('ó', 'o');
            ret = ret.Replace('ę', 'e');
            ret = ret.Replace('ś', 's');
            ret = ret.Replace('ł', 'l');
            ret = ret.Replace('ń', 'n');
            ret = ret.ToLower();
            return ret;
        }
    }
}
