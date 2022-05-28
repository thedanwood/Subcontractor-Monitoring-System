using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class ChecklistResponseRadiosViewModel
    {
        public int QuestionId { get; set; }
        public int SubSectionId { get; set; }
        public int ChecklistPageOriginEnum { get; set; }
    }
}