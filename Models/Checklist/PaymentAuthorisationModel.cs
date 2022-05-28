using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class PaymentAuthorisationModel
    {
        public int SubSectionID { get; set; }
        public bool isComplete { get; set; }
        public bool isEditable { get; set; }
        public bool completionIsDisabled { get; set; }
        public string SignatureName { get; set; }
        public string FullName { get; set; }
        public byte[] SignatureImage { get; set; }
        public DateTime SignedDateTime { get; set; }
    }
}