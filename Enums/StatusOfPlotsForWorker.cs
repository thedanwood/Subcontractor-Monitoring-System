using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum StatusOfPlotsForWorker
    {
        [Description("Not All Sections Have Been Completed")]
        NotAllComplete = 1,
        [Description("All Sections Have Been Completed")]
        AllComplete = 2,
    }
}