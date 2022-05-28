using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfTradeContractorChecklistQuestionStaffResponse
    {
        [Description("Incomplete")]
        Incomplete = 0,
        [Description("Checklist returned as satisfactory")]
        Satisfactory = 1,
        [Description("Checklist returned with minor additonal works outlined")]
        RejectedMinor = 2,
        [Description("Checklist returned with major defects with the works outlined")]
        RejectedMajor = 3,
    }
}