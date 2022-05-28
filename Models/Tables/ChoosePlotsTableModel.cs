using BQCDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Tables
{
    public class ChoosePlotsTableModel
    {
        public string EncryptedPlotID { get; set; }
        public int PlotID { get; set; }
        public int PlotNumber { get; set; }
        //public int HouseType { get; set; }
        //public string CurrentBuildSection { get; set; }
        public string BuildStagesCompleted { get; set; }
        public string PlotName { get; set; }
        public string BuildSectionsCompleted { get; set; }
        public int Status { get; set; }
        public List<SectionDetailsForPlotsTable> SectionDetailsForPlotsTables { get; set; }
    }
}