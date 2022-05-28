using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfTradeContractorSubSection
    {
        [Description("Incomplete by Trade Contractor")]
        IncompleteByTradeContractor = 1,
        [Description("Not been approved by Site Management and amendment is needed")]
        AmendmentByTradeContractorNeeded = 2,
        [Description("Completed and approved by Site Management but payment not authorised")]
        CompletedAndApprovedByStaffButPaymentNotAuthorised = 3,
        [Description("Awaiting response from staff")]
        AwaitingResponseFromStaff = 4,
        [Description("Completed and approved by Site Management and payment authorised")]
        CompletedAndApprovedByStaffAndPaymentAuthorised = 5,
        [Description("Trade contractor checklist not ready to be completed")]
        SubSectionDependenciesIncompleted = 6,
    }
}