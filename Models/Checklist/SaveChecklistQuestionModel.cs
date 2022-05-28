using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SaveChecklistQuestionModel
    {
        public string Note { get; set; }
        public int QuestionID { get; set; }
        public int ApproverResponseEnum { get; set; }
    }
}