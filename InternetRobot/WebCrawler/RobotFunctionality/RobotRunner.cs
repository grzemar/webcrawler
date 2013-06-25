using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace WebCrawler
{
    /// <summary> Provides methods for crawling pages by one thread.
    /// </summary>
    public class RobotRunner : IRobotRunner
    {
        private IRobotFilter robotFilter;
        private RobotManager robotManager;
        private Document currentDocument;
        private WebClient client;
        const long MAX_SIZE_FOR_ADDRESS_SEARCHING = 50000000;

        /// <summary> Initializes a new RobotRunner instance and sets its RobotManager and filter.
        /// </summary>
        public RobotRunner(IRobotFilter filter, RobotManager manager)
        {
            robotFilter = filter;
            robotManager = manager;
            client = new WebClient();
        }

        /// <summary> Crawls pages until specific conditions happen.
        /// Downloads and saves pages to temp folder and searches pages for new URLs to crawl.
        /// </summary>
        public void CrawlPage()
        {
            bool operate = true;
            bool working = true;
            while (operate)
            {
                Document doc = robotManager.GetPageFromCollection();
                if (doc!=null)
                {
                    if (working == false)
                    {
                        working = true;
                        robotManager.ChangeWorkingThreadsNumber(1);
                    }
                    string discAddress = 
                        Path.Combine(Path.GetTempPath(), doc.HttpAddress.EscapeUrl()).Replace("\\","/");
                    bool downloaded = DownloadDocument(doc.HttpAddress,discAddress);
                    doc.DiscAddress = discAddress;
                    currentDocument = doc;
                    if (downloaded) FindUrisInDocument(discAddress);
                }
                else
                {
                    if (working == true)
                    {
                        int num = robotManager.ChangeWorkingThreadsNumber(-1);
                        working = false;
                        if (num == 0) operate = false;
                    }
                }
            }
        }

        private bool DownloadDocument(string address, string discAddress)
        {
            try
            {
                client.DownloadFile(address, discAddress);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary> Checks for Uris in downloaded document.
        /// </summary>
        public void FindUrisInDocument(string address)
        {
            FileInfo f = new FileInfo(address);
            if (f.Length < MAX_SIZE_FOR_ADDRESS_SEARCHING)
            {
                using (StreamReader sr = new StreamReader(address))
                {
                    string result = sr.ReadToEnd();
                    Regex urlRx = 
                        new Regex(@"(https?|file)\://[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", 
                            RegexOptions.IgnoreCase);
                    MatchCollection matches = urlRx.Matches(result);
                    foreach (Match match in matches)
                    {
                        if (robotFilter.CanBeCrawled(match.Value) == true)
                        {
                            Document doc = robotManager.AddPageToCollection(match.Value);
                            currentDocument.AddNeighbour(doc);
                        }
                    }
                }
            }
        }
    }
}
