using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.Enums
{
    public enum DependencyTypes
    {
        ReliantOnCompletion=1,
        ReliantOnCompletionOrOtherCompletion=2, //reliant on the dependent section being completed, or another section with same type
    }
}