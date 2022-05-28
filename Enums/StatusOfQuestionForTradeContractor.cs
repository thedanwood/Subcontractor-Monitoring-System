using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfQuestionForTradeContractor
    {
        [Description("Awaiting Trade Contractor Completion")]
        AwaitingTradeContractorCompletion = 0,
        [Description("Awaiting Site Management Response")]
        AwaitingStaffResponse = 1,
        [Description("Approved By Site Management")]
        Approved = 2,
        [Description("Marked as Conditional By Site Management")]
        Conditional = 3,
    }
}