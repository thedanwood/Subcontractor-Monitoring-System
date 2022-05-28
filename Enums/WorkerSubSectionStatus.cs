using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.Enums
{
    public enum EnumWorkerSubSectionStatus
    {
        [Description("Awaiting Subsequent Site Management Section Completion")]
        AwaitingSubsequentSiteCompletion = 1,
        [Description("Incomplete By Sub Contractor")]
        IncompleteByWorker = 2,
        [Description("Awaiting Response From Site Management")]
        AwaitingResponseFromSite = 3,
        [Description("Not Been Approved By Site And Amendment Is Needed")]
        AmendmentByWorkerNeeded = 4,
        [Description("Completed And Approved By Site Management")]
        CompletedAndApprovedBySite = 5,
    }
}