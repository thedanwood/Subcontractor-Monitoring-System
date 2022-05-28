using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum EnumStatusOfSubContractorResponse
    {
        [Description("Incomplete")]
        Incomplete = 0,
        [Description("Awaiting Site Response")]
        AwaitingSiteResponse = 1,
        [Description("Approved By Site Management")]
        Approved = 2,
    }
}