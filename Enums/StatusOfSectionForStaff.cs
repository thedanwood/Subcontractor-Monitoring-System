using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfSectionForStaff
    {
        [Description("Not yet been completed by Trade Contractor")]
        NotYetBeenCompletedByTradecontractor = 1,
        [Description("Completed by Trade Contractor but not yet approved by Site Management")]
        CompletedByTradeContractorButNotReviewedByStaff = 2,
        [Description("Completed by Trade Contractor and approved by Site Management but payment not authorised")]
        CompletedByTradeContractorAndReviewedByStaffButPaymentNotAuthorised = 3,
        [Description("Completed by Trade Contractor and approved by Site Management and payment authorised")]
        CompletedByTradeContractorAndReviewedByStaffAndPaymentAuthorised = 4,
    }
}