using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class ChecklistIndexValues
    {
        public int MostRecentAttemptIndexValue { get; set; }
        public int MostRecentAttachedAcceptorFilePathIndexValue { get; set; }
        public int MostRecentAttachedApproverFilePathIndexValue { get; set; }
        public QuestionAttemptResponseInfo MostRecentAttemptResponseInfo { get; set; }
    }
}