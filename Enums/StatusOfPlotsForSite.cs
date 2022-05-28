using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum EnumStatusOfPlotForSite
    {
        [Description("Not yet been completed by Sub Contractor")]
        NotYetBeenCompletedBySubContractor = 1,
        [Description("Completed by Sub Contractor but not yet approved by Site Management")]
        CompletedBySubContractorButNotApprovedBySite = 2,
        [Description("Completed by Sub Contractor and approved by Site Management")]
        CompletedBySubContractorAndApprovedBySite = 3,
    }
}