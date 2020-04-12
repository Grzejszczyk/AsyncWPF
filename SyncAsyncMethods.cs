using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncWPF
{
    public partial class MainWindow
    {
        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");
            return output;
        }
        private void RunDownloadSync()
        {
            List<string> webSites = PrepData();
            foreach (string site in webSites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }
        private void RunDownloadParallelSync(IProgress<ProgresReportModel> progress, CancellationToken ct)
        {
            List<string> webSites = PrepData();
            ProgresReportModel report = new ProgresReportModel();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            Parallel.ForEach<string>(webSites, (site) =>
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
                ct.ThrowIfCancellationRequested();

                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100) / webSites.Count;

                progress.Report(report);
            });
            PrintResults(output);
        }
        private async Task RunDownloadAsync(IProgress<ProgresReportModel> progress, CancellationToken ct)
        {
            List<string> webSites = PrepData();
            ProgresReportModel report = new ProgresReportModel();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string site in webSites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));

                output.Add(results);

                ct.ThrowIfCancellationRequested();

                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100) / webSites.Count;

                progress.Report(report);
            }
        }
        private async Task RunDownloadParallelAsync()
        {
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            List<string> webSites = PrepData();

            foreach (string site in webSites)
            {
                tasks.Add(Task.Run(() => DownloadWebsite(site)));
            }

            var results = await Task.WhenAll(tasks);

            resultsTextBlock.Text = "";
            foreach (var item in results)
            {
                ReportWebsiteInfo(item);
            }
        }
        private async Task RunDownloadParallelAsyncV2(IProgress<ProgresReportModel> progress, CancellationToken ct)
        {
            List<string> webSites = PrepData();
            ProgresReportModel report = new ProgresReportModel();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            await Task.Run(() =>
            {
                Parallel.ForEach<string>(webSites, (site) =>
                {
                    WebsiteDataModel results = DownloadWebsite(site);
                    output.Add(results);
                    ct.ThrowIfCancellationRequested();

                    report.SitesDownloaded = output;
                    report.PercentageComplete = (output.Count * 100) / webSites.Count;

                    progress.Report(report);
                });
            });

            PrintResults(output);
        }
        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();
            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);
            return output;
        }
    }
}
