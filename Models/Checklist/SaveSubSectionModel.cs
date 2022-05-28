using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SaveSubSectionModel
    {
        public List<SaveChecklistQuestionModel> ChecklistQuestionResponses { get; set; }
        public string PrintName { get; set; }
        public byte[] Signature { get; set; }
        public int SubSectionID { get; set; }
        public int PlotID { get; set; }
    }
}