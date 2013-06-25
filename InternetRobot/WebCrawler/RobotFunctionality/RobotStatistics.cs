using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebCrawler
{
    /// <summary> Provides methods for managing statistics from crawling.
    /// </summary>
    public class RobotStatistics : IRobotStatistics
    {
        List<Document> list;

        /// <summary> Initializes a new RobotRunner instance 
        /// </summary>
        public RobotStatistics(List<Document> list)
        {
            this.list = list;
        }

        /// <summary> Occurs when statistics are saved.
        /// </summary>
        public event EventHandler WorkFinishedHandler;

        /// <summary> Saves statistics asynchronously to file.
        /// </summary>
        public void Save(string fileName)
        {
            Action<object> action = (object path) =>
            {
                SaveToCsv((string)path);
            };
            Task.Factory.StartNew(action, (object)fileName);
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

        /// <summary> Saves statistics synchronously to CSV file.
        /// </summary>
        public void SaveToCsv(string fileName)
        {
            foreach (Document d in list)
            {
                d.Id = d.HttpAddress.EscapeUrl();
            }
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (Document d in list)
                {
                    sw.Write(d.Id);
                    foreach (Document neighbour in d.Neighbours)
                    {
                        sw.Write(';');
                        sw.Write(neighbour.Id);
                    }
                    sw.WriteLine();           
                }
            }
            OnWorkFinished(EventArgs.Empty);
        }
    }
}