using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace WebCrawler
{
    /// <summary> Provides methods for managing robot threads.
    /// </summary>
    public class RobotManager : IRobotManager
    {
        private Queue<WebDocument> addressQueue;
        private List<WebDocument> finishedAddresses;
        private volatile int workingThreads;
        private List<Thread> threadList;

        private string downloadPath;

        /// <summary> Occurs when all robot work is finished.
        /// </summary>
        public event EventHandler WorkFinishedHandler;

        /// <summary> Initializes a new RobotManager instance.
        /// </summary>
        public RobotManager(string downloadPath)
        {
            addressQueue = new Queue<WebDocument>();
            finishedAddresses = new List<WebDocument>();
            threadList = new List<Thread>();

            this.downloadPath = downloadPath;
        }

        /// <summary> Raises workfinished event.
        /// </summary>
        protected virtual void OnWorkFinished(EventArgs e)
        {
            EventHandler handler = WorkFinishedHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary> Returns domain of specified web URL.
        /// </summary>
        public string DomainAddress(string address)
        {
            Uri uri;
            if (address.Contains("://"))
                uri = new Uri(address);
            else uri = new Uri("http://" + address);
            return uri.Host;
        }

        /// <summary> Starts robot work on specified address.
        /// Starts robot threads execution. Downloads filter file and creates filters and runners.
        /// </summary>
        public void StartRobot(int threadNumber, string address)
        {
            workingThreads = threadNumber;
            string hostAddress = DomainAddress(address);
            string robotAddress = "http://" + hostAddress + "/robots.txt";
            string localAddress = Path.Combine(downloadPath, "robotsInternetRobot.txt");
            if (File.Exists(localAddress))
                File.Delete(localAddress);
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(robotAddress, localAddress);
            }
            catch (WebException)
            {
                using (File.Create(localAddress)) { }
            }
            threadList.Clear();
            IRobotFilter filter = new RobotFilter(localAddress, hostAddress);
            for (int i = 0; i < workingThreads; i++)
            {
                IRobotRunner runner = new RobotRunner(filter, this, downloadPath);
                Thread thread = new Thread(runner.CrawlPage);
                threadList.Add(thread);
            }
            string acceptableAddress;
            if (!address.Contains("://"))
                acceptableAddress = "http://" + hostAddress;
            else acceptableAddress = address;
            if (filter.CanBeCrawled(address) == true) AddPageToCollection(acceptableAddress);
            foreach (Thread thread in threadList)
            {
                thread.Start();
            }
        }

        /// <summary> Stops robot. Stops robot threads execution.
        /// </summary>
        public void StopRobot()
        {
            foreach (Thread t in threadList)
            {
                if (t.IsAlive)
                    t.Abort();
            }
            while (addressQueue.Count > 0)
            {
                GetPageFromCollection();
            }
            RemoveEmptyDocs();
        }

        private void RemoveEmptyDocs()
        {
            foreach (WebDocument d in finishedAddresses)
            {
                d.Neighbours.RemoveAll((document) => document.DiscAddress == null);
            }
            finishedAddresses.RemoveAll((document) => document.DiscAddress == null);
        }

        /// <summary> Adds page from specified URL to collection, returning its Document.
        /// </summary>
        public WebDocument AddPageToCollection(string address)
        {
            lock (this)
            {
                string addressNormalized = address.NormalizeForUrl();
                WebDocument doc = new WebDocument(addressNormalized);
                WebDocument resultUnfinished = addressQueue.FirstOrDefault(
                    (document) => { return document.Equals(doc); });
                WebDocument resultFinished = finishedAddresses.FirstOrDefault(
                    (document) => { return document.Equals(doc); });
                if (resultUnfinished == null && resultFinished == null)
                    addressQueue.Enqueue(doc);
                else if (resultUnfinished != null) return resultUnfinished;
                else return resultFinished;
                return doc;
            }
        }

        /// <summary> Gets one page from collection, returning its Document.
        /// </summary>
        public WebDocument GetPageFromCollection()
        {
            lock (this)
            {
                if (addressQueue.Count > 0)
                {
                    WebDocument doc = addressQueue.Dequeue();
                    finishedAddresses.Add(doc);
                    return doc;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary> Changes number of working threads. Called from runner thread.
        /// </summary>
        public int ChangeWorkingThreadsNumber(int count)
        {
            lock (this)
            {
                int c = count;
                if (c < -1) c = -1;
                if (c > 1) c = 1;
                workingThreads += c;
                if (workingThreads < 0) workingThreads = 0;
                if (workingThreads == 0)
                {
                    OnWorkFinished(EventArgs.Empty);
                }
                return workingThreads;
            }
        }

        /// <summary> Returns list of already crawled documents.
        /// </summary>
        public List<WebDocument> FinishedAddresses()
        {
            return finishedAddresses;
        }
    }
}