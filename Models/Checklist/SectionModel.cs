using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SectionModel 
    {
        public List<SubSectionInfoForButtonsModel> SubSectionInfoForButtons { get; set; }
        public List<SectionNamingInformationModel> SectionNaming { get; set; }
        public SiteAndPlotInformationModel SitePlotInfo { get; set; }
        public string SectionCategoryName { get; set; }
        public int SubSectionIDToLoad { get; set; }
        public int SectionIDToLoad { get; set; }
    }
}