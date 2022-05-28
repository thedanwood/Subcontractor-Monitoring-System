using BQC.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.ViewModels
{
    public class ChoosePlotsViewModel
    {
        public UserAccessRightInfo userAccessInfo { get; set; }
        public int SiteID { get; set; }
    }
}