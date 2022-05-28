using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfStaffSubSection
    {
        [Description("Completed")]
        Complete = 1,
        [Description("Not yet completed")]
        Incomplete = 2,
        [Description("Sub Section Dependencies Incompleted")]
        SubSectionDependenciesIncompleted =3
    }
}