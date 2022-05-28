using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SubSectionInfoForButtonsModel
    {
        public int SubSectionID { get; set; }
        public string SubSectionButtonName { get; set; }
        public int SubSectionStatusForButtonState { get; set; }
        public int SectionID { get; set; }
    }
}