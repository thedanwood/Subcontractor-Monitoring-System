using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SiteAndPlotInformationModel
    {
        public SiteInformationModel SiteInfo { get; set; }
        public PlotInformationModel PlotInfo { get; set; }
    }
}