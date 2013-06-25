using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace WebCrawler
{
    /// <summary> Provides methods for filtering URLs crawled by robot.
    /// </summary>
    public class RobotFilter : IRobotFilter
    {
        private string robotsTxtAddress;
        private string hostAddress;
        private List<string> rules;

        /// <summary> Initializes a new RobotFilter instance and sets its filters.
        /// </summary>
        public RobotFilter(string filterAddress, string domainAddress)
        {
            robotsTxtAddress = filterAddress;
            string[] splitter = domainAddress.Split('.');
            if (splitter[0].Equals("www"))
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 1; i < splitter.Length; i++)
                {
                    builder.Append(splitter[i]);
                    if (i < splitter.Length - 1) builder.Append('.');
                }
                hostAddress = builder.ToString();
            }
            else hostAddress = domainAddress;
            rules = new List<string>();
            ReadRobotsTxt();
        }

        private void ReadRobotsTxt()
        {
            using (StreamReader reader = new StreamReader(robotsTxtAddress))
            {
                bool ignoreMode = true;
                string line;
                while((line = reader.ReadLine()) !=null)
                {
                    if (line.Contains("User-agent") && (line.Contains("*")||line.Contains("grzemar")))
                        ignoreMode = false;
                    else if (ignoreMode == false)
                    {
                        if (line.Contains("Disallow: "))
                        {
                            string[] s = line.Split(' ');
                            s[1] = s[1].Trim();
                            s[1] = s[1].Trim('*');
                            rules.Add(s[1]);
                        }
                        else if (line.Contains("User-agent"))
                            ignoreMode = true;
                    }
                }
            }
        }

        /// <summary> Determines if address can be crawled based on robots.txt rules loaded into this class.
        /// </summary>
        public bool CanBeCrawled(string address)
        {
            if (!address.Contains(hostAddress)) return false;
            foreach (string rule in rules)
            {
                if (!rule.Equals(String.Empty))
                {
                    string creation;
                    if (rule.StartsWith("/"))
                        creation = hostAddress + rule;
                    else creation = rule;
                    Regex reg = new Regex(creation);
                    if (reg.Matches(address).Count > 0) return false;
                }
            }
            return true;
        }

    }
}
