using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfPlot
    {
        [Description("All sections completed by Trade Contractor and approved by Site Management")]
        AllSectionsCompletedByTradeContractorAndApprovedBySiteManagement = 1,
        [Description("Not all sections completed by Trade Contractor and approved by Site Management")]
        NotAllSectionsCompletedByTradeContractorAndApprovedBySiteManagement = 2,
    }
}