using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class QuestionResponseInfoViewModel
    {
        public int ChecklistPageOriginEnum { get; set; }
        public int ChecklistPageOriginTypeEnum { get; set; }
        public QuestionAndResponseInfoModel QuestionAndResponseInfo { get; set; }
    }
}