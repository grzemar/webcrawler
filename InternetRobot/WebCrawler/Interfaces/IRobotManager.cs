namespace WebCrawler
{
    /// <summary> Defines methods for starting and stopping internet robot.
    /// </summary>
    public interface IRobotManager
    {
        /// <summary> Starts working threads and sets their parameters.
        /// </summary>
        void StartRobot(int threadNumber, string address);
        /// <summary> Stops working threads.
        /// </summary>
        void StopRobot();
    }
}
