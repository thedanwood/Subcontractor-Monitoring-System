using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.ViewModels
{
    public class GetPlotInformationViewModel
    {
        public int PlotID { get; set; }
        public int BQCSectionID { get; set; }
        public int SubSectionTypeEnum { get; set; }
        public int QuestionsCount { get; set; }
        public int ResponsesCount { get; set; }
    }
}