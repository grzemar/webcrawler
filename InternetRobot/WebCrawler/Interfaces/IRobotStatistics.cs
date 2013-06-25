namespace WebCrawler
{
    /// <summary> Defines methods for statistics saving.
    /// </summary>
    public interface IRobotStatistics
    {
        /// <summary> Saves statistics in file specified by path.
        /// </summary>
        void Save(string fileName);
    }
}
