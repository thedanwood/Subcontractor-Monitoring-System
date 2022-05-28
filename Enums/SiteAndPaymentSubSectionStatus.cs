using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;

namespace BQC.Enums
{
    public enum EnumSiteAndPaymentSubSectionStatus
    {
        [Description("Incomplete")]
        Incomplete = 1,
        [Description("Complete")]
        Complete = 2,
    }
}