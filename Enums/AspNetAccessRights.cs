using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum AspNetAccessRights
    {
        [Description("Croudace Site Staff")]
        CroudaceSiteStaff = 1,
        [Description("Croudace Staff")]
        CroudaceStaff = 2,
        [Description("Tradecontractor")]
        Tradecontractor = 2,
    }
}