﻿using System;
using System.Collections.Generic;
using System.Windows;
using WebAnalyzer.Interfaces;
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
        const int MAX_THREAD_COUNT = 50;
        RobotFinishedCallBack callBack;
        List<WebDocument> documents;
        private string downloadPath = "C:\\crawler\\9-14-2013";

        public MainWindow()
        {
            InitializeComponent();
            runButton.IsEnabled = true;
            saveButton.IsEnabled = false;
            stopButton.IsEnabled = false;
            documents = new List<WebDocument>();
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

            downloadPath = downloadDirectoryText.Text;
            if (downloadPath == "")
            {
                downloadPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "crawler");
            }

            downloadPath = System.IO.Path.Combine(downloadPath, DateTime.Now.ToString("yyyy/d/M/HH/mm/ss"));


            if (!System.IO.Directory.Exists(downloadPath))
                System.IO.Directory.CreateDirectory(downloadPath);

            this.downloadDirectoryText.Text = downloadPath;

            subHeaderText.Text += "Download documents to - " + downloadPath;

            int threadCount = 1;
            SetRunMode();
            robotManager = new RobotManager(downloadPath);
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
            robotManager.StartRobot(threadCount, addressTextBox.Text);
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
            bool? result = dlg.ShowDialog();
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

        private void analyzyButton_Click(object sender, RoutedEventArgs e)
        {
            IDomunetsAnalyzer analyzer = new WebAnalyzer.Analyzer();
            analyzer.Analyze(this.downloadPath);
        }
    }
}
