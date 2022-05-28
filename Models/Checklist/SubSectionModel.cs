using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SubSectionModel
    {
        public string SectionFullName { get; set; }
        public List<QuestionAndResponseInfoModel> ListQuestionAndResponseInfo { get; set; }
        public int SectionID { get; set; }
        public int SubSectionID { get; set; }
        public string SignatureName { get; set; }
        public byte[] SignatureImage { get; set; }
        public DateTime SignatureDateTime { get; set; }
        public int SubSectionStatusEnumValue { get; set; }
        public string SubSectionStatusString { get; set; }
        public bool isComplete { get; set; }
        public bool isEditable { get; set; }
        //for signature pad name population
        public string CurrentUserFullName { get; set; }
    }
}