using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.Models.Checklist
{
    public class GetPlotInformationForSiteModel
    {
        public int PlotID { get; set; }
        public int BQCSectionID { get; set; }
        public bool IsComplete { get; set; }
        public int BuildStagesCompleted { get; set; }
    }
}