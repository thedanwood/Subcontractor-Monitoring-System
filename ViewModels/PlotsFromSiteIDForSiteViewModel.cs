using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.ViewModels
{
    public class PlotsFromSiteIDForSiteViewModel
    {
        public int PlotID { get; set; }
        public int PlotNo { get; set; }
        //public int HouseType { get; set; }
        public string CurrentBuildSection { get; set; }
        public int Status { get; set; }
    }
}