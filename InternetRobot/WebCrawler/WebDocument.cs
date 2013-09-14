using System;
using System.Collections.Generic;

namespace WebCrawler
{
    /// <summary> Represents document downloaded from web.
    /// </summary>
    public class WebDocument : IEquatable<WebDocument>
    {
        /// <summary> Web address of document.
        /// </summary>
        public string HttpAddress
        {
            get;
            set;
        }

        /// <summary> Local address of document.
        /// </summary>
        public string DiscAddress
        {
            get;
            set;
        }

        /// <summary> Id of document.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary> List of documents of web addresses found in this document.
        /// </summary>
        public List<WebDocument> Neighbours 
        {
            get; 
            private set;
        }

        /// <summary> Initializes new document with a specified web address.
        /// </summary>
        public WebDocument(string httpAddress)
        {
            Neighbours = new List<WebDocument>();
            HttpAddress = httpAddress;
        }

        /// <summary> Adds new document with address found in this document to list.
        /// </summary>
        public void AddNeighbour(WebDocument desc)
        {
            lock (this)
            {
                Neighbours.Add(desc);
            }
        }

        /// <summary> IEquatable Method for comparing document with other documents basing on their web addresses.
        /// </summary>
        public bool Equals(WebDocument other)
        {
            if (Object.ReferenceEquals(other, null)) return false;

            if (Object.ReferenceEquals(this, other)) return true;

            return other.HttpAddress.Equals(this.HttpAddress);
        }

    }
}
