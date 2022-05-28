using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SubSectionAndSectionNamingEnumValuesModels
    {
        public int SectionCategoryEnum { get; set; }
        public int SubSectionCategoryEnum { get; set; }
        public int? SubSectionRoleEnumValue { get; set; }
    }
    public class SubSectionAndSectionStrings
    {
        public string SectionCategoryName { get; set; }
        public string SubSectionCategoryName { get; set; }
        public string SubSectionRoleNameEnum { get; set; }
        public string FullSectionName { get; set; }
    }
    public class TotalSectionsCompletedValueAndString
    {
        public int TotalSectionsValue { get; set; }
        public int TotalSectionsCompletedValue { get; set; }
        public string TotalSectionsCompletedString { get; set; }
    }
    //public class SectionNamingInfoForChecklistDropdown
    //{
    //    public int SectionNumber { get; set; }
    //    public string SectionFullName { get; set; }
    //    //public string SectionWorkerName { get; set; }
    //    //public string SubSectionName { get; set; }
    //}
}