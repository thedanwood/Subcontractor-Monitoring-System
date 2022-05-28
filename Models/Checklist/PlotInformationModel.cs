using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class PlotInformationModel
    {
        public string EncryptedPlotID { get; set; }
        public int PlotID { get; set; }
        public int? PlotNumber { get; set; }
        public string PlotName { get; set; }
    }
}