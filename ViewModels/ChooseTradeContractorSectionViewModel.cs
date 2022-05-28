using BQC.Models.Checklist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.ViewModels
{
    public class ChooseTradeContractorSectionViewModel
    {
        public SiteAndPlotInformationModel SitePlotInfo { get; set; }
        public string RoleNameEnum { get; set; }
    }
}