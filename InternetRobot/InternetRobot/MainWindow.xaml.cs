using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebCrawler;

namespace InternetRobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    delegate void RobotFinishedCallBack();
    public partial class MainWindow : Window
    {
        RobotManager robotManager;
        const int MAX_THREAD_COUNT = 12;
        RobotFinishedCallBack callBack;
        List<Document> documents;

        public MainWindow()
        {
            InitializeComponent();
            runButton.IsEnabled = true;
            saveButton.IsEnabled = false;
            stopButton.IsEnabled = false;
            documents = new List<Document>();
            callBack = this.UpdateWhenStopped;
        }

        private void SetStillMode()
        {
            runButton.IsEnabled = true;
            saveButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }

        private void SetRunMode()
        {
            runButton.IsEnabled = false;
            saveButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private void WorkFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(callBack);
        }

        private void SaveFinished()
        {
            subHeaderText.Text = "Statistics saving finished.";
            SetStillMode();
        }

        private void UpdateWhenStopped()
        {
            robotManager.StopRobot();
            subHeaderText.Text = "Work finished. You may save statistics to file.";
            SetStillMode();   
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            subHeaderText.Text = "Robot is working, please wait.";
            int threadCount = 1;
            SetRunMode();
            robotManager = new RobotManager();
            callBack = this.UpdateWhenStopped;
            robotManager.WorkFinishedHandler += WorkFinished;
            try
            {
                threadCount = Convert.ToInt32(threadTextBox.Text);
            }
            catch (OverflowException) { }
            catch (FormatException) { }
            if (threadCount < 1)
            {
                threadCount = 1;
                threadTextBox.Text = "1";
            }
            if (threadCount > MAX_THREAD_COUNT)
            {
                threadCount = MAX_THREAD_COUNT;
                threadTextBox.Text = MAX_THREAD_COUNT.ToString();
            }
            robotManager.StartRobot(threadCount,addressTextBox.Text);
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            robotManager.StopRobot();
            UpdateWhenStopped();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Csv documents (.csv)|*.csv";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                subHeaderText.Text = "Saving statistics...";
                SetRunMode();
                stopButton.IsEnabled = false;
                string fileName = dlg.FileName;
                documents = robotManager.FinishedAddresses();
                RobotStatistics stats = new RobotStatistics(documents);
                callBack = this.SaveFinished;
                stats.WorkFinishedHandler += WorkFinished;
                stats.Save(fileName);
            }
        }
    }
}
