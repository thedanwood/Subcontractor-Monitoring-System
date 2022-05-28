using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class AddCommentsOrFilesModel
    {
        public SiteAndPlotInformationModel SitePlotInfo { get; set; }
        public int SubSectionID { get; set; }
        public int Index { get; set; }
        public int QuestionID { get; set; }
        public int CurrentQuestionAttemptCount { get; set; }
        public int QuestionAttemptCountForNewFileSave { get; set; }
        public int AcceptorOrApproverEnum { get; set; }
    }
}