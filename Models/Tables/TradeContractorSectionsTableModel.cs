using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Tables
{
    public class TradeContractorSectionsTableModel
    {
        public int SectionID { get; set; }
        public string FullSectionNameString { get; set; }
        public string TotalBuildSectionsCompleted { get; set; }
        public int Status { get; set; }
    }
}