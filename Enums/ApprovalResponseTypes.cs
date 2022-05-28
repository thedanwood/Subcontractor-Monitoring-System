using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum ApprovalResponseTypes
    {
        [Description("Incomplete")]
        [Display(Name="Incomplete")]
        Incomplete = 0,
        [Description("Checklist returned as satisfactory")]
        [Display(Name = "Approve")]
        Approve = 1,
        [Description("Checklist returned with minor additonal works outlined (conditional)")]
        [Display(Name = "Conditional")]
        SubjectToConditions = 2,
        [Description("Checklist returned with major defects with the works outlined (rejected)")]
        [Display(Name = "Reject")]
        Reject = 3,
    }
}