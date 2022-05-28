using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum SectionCategoryNames
    {
        [Description("Foundation")]
        Foundation = 1,
        [Description("Oversite to DPC")]
        OversiteToDPC = 2,
        [Description("Superstructure")]
        Superstructure = 3,
        [Description("Internals 1st Fix")]
        InternalsFirstFix = 4,
        [Description("Internals Drylining")]
        InternalsDrylining = 5,
        [Description("Internals 2nd Fix")]
        InternalsSecondFix = 6,
        [Description("Internals Painting")]
        InternalsPainting = 7,
        [Description("Internals Finals")]
        InternalsFinals = 8,
        [Description("Externals")]
        Externals = 9,
    }
}