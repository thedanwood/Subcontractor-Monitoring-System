using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Email
{
    public class EmailSendingInfoModel
    {
        public string Link { get; set; }
        public List<string> SiteEmails { get; set; }
        public List<string> TradeContractorEmails { get; set; }
        public string SiteName { get; set; }
        public string PlotName { get; set; }
        public string SubSectionName { get; set; }
        public int SiteID { get; set; }
        public int PlotID { get; set; }
        public int SubSectionID { get; set; }
    }
}