using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BQC.Models.Checklist;

namespace BQC.ViewModels
{
    public class SectionDropdownViewModel
    {
        public SiteAndPlotInformationModel SitePlotInfo { get; set; }
        public List<SectionNamingInformationModel> SectionNamingInfo { get; set; }
    }
}