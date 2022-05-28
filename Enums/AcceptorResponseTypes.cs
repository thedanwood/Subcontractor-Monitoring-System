using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum AcceptorResponseTypes
    {
        [Description("Incomplete")]
        Incomplete = 0,
        [Description("Accepted")]
        Accepted = 1,
    }
}