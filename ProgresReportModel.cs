using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncWPF
{
    public class ProgresReportModel
    {
        public List<WebsiteDataModel> SitesDownloaded { get; set; } = new List<WebsiteDataModel>();
        public int PercentageComplete { get; set; } = 0;
        public int TotalScope { get; set; } = 0;
    }
}
