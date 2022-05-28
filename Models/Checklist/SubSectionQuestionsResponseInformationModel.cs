using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SubSectionQuestionsResponseInformationModel
    {
        public int TotalQuestionsInSubSection { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalQuestionsApproved { get; set; }
        public int TotalQuestionsMarkedAsConditional { get; set; }
        public int TotalQuestionsRejected { get; set; }
        public int TotalQuestionsResponded { get; set; }
    }
}