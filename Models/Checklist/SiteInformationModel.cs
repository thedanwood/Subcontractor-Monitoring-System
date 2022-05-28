using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SiteInformationModel
    {
        public int SiteID { get; set; }
        public string EncryptedSiteID { get; set; }
        public string SiteName { get; set; }
        public string MarketingName { get; set; }
    }
}