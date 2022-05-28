using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class ChecklistModel
    {
        public int SectionID { get; set; }
        public List<SectionNamingInformationModel> SectionNaming { get; set; }
        public SubSectionModel SubSectionModel { get; set; }
        public PaymentAuthorisationModel PaymentAuthModel { get; set; }
        public SiteAndPlotInformationModel SiteAndPlotInfo { get; set; }
    }
}