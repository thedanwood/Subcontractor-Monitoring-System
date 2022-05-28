using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class SignaturePadModel
    {
        public int SubSectionID { get; set; }
        public bool isEditable { get; set; }
        public byte[] SignatureImage { get; set; }
        public string SignedBy { get; set; }
        public DateTime SignedDateTime { get; set; }
        public string FullName { get; set; }
    }
}