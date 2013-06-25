namespace WebCrawler
{
    /// <summary> Defines methods for crawling pages and finding urls in crawled pages.
    /// </summary>
    public interface IRobotRunner
    {
        /// <summary> Thread method.
        /// </summary>
        void CrawlPage();

        /// <summary> Finds uris in crawled page.
        /// </summary>
        void FindUrisInDocument(string address);
    }
}
