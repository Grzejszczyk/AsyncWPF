using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RunDownloadSync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsTextBlock.Text += $"Total elapsed time is: { elapsedMs }";
        }
        private void executeParallelSync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgresReportModel> progress = new Progress<ProgresReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            RunDownloadParallelSync(progress, cts.Token);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsTextBlock.Text += $"Total elapsed time is: { elapsedMs }";
        }
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgresReportModel> progress = new Progress<ProgresReportModel>();
            progress.ProgressChanged += ReportProgress;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await RunDownloadAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {
                resultsTextBlock.Text += $"Operation Cancelled. {Environment.NewLine}";
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsTextBlock.Text += $"Total elapsed time is: { elapsedMs }";
        }
        private async void executeAsyncParallel_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await RunDownloadParallelAsync();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            resultsTextBlock.Text += $"Total elapsed time is: { elapsedMs }";
        }
        private async void executeAsyncParallelV2_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgresReportModel> progress = new Progress<ProgresReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            await RunDownloadParallelAsyncV2(progress, cts.Token);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsTextBlock.Text += $"Total elapsed time is: { elapsedMs }";
        }
        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
        

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsTextBlock.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long: {Environment.NewLine}";
        }
        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsTextBlock.Text = "";
            foreach (var item in results)
            {
                resultsTextBlock.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long: {Environment.NewLine}";
            }
        }
        private void ReportProgress(object sender, ProgresReportModel e)
        {
            progressBar.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }
    }
}
