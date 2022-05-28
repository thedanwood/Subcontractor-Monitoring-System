using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class AddFileFormModel
    {
        public int QuestionID { get; set; }
        public int PlotID { get; set; }
        public int SubSectionID { get; set; }
        public int QuestionCurrentFilesAddedCount { get; set; }
        public int QuestionAttemptCountForNewFileSave { get; set; }
        public int AcceptorOrApproverEnum { get; set; }
    }
}