namespace WebCrawler
{
    /// <summary> Defines methods for filtering URLs crawled by robot.
    /// </summary>
    public interface IRobotFilter
    {
        /// <summary> Determines if address can be crawled.
        /// </summary>
        bool CanBeCrawled(string address);
    }
}
