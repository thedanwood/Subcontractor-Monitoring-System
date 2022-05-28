using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Checklist
{
    public class QuestionAcceptAndResponseEnums
    {
        public int QuestionAcceptorResponseEnum { get; set; }
        public int QuestionAcceptorResponseString { get; set; }
        public int QuestionApproverResponseEnum { get; set; }
        public string QuestionApproverResponseString { get; set; }
        public int QuestionStatusEnumForTradeContractor { get; set; }
        public string QuestionStatusStringForTradeContractor { get; set; }
        public string QuestionApprovalAlertColor { get; set; }
        public string QuestionAcceptorAlertColor { get; set; }
    }
    public class QuestionAndResponseInfoModel : QuestionAcceptAndResponseEnums
    {
        public ChecklistIndexValues ChecklistIndexValues { get; set; }
        public List<QuestionAttemptResponseInfo> QuestionAttemptResponseInfo { get; set; }
        public int QuestionAttemptCount { get; set; }
        public int QuestionAttemptCountForFileSave { get; set; }
        public int QuestionStatusForRoleEnum { get; set; }
        public string QuestionContent { get; set; }
        public int ChecklistQuestionID { get; set; }
    }
    public class QuestionAttemptResponseInfo
    {
        public int AttemptNumber { get; set; }
        public AcceptorInfo AcceptorInfo { get; set; }
        public ApproverInfo ApproverInfo { get; set; }
    }
    public class AcceptorInfo : AcceptorOrApproverInfo
    {
        public bool AcceptorAcknowledged { get; set; }
        public List<string> AcceptorFilePaths { get; set; }
    }
    public class ApproverInfo : AcceptorOrApproverInfo
    {
        public string ApproverName  { get; set; }
        public int ApproverResponseEnum { get; set; }
        public string ApproverResponseString { get; set; }
        public List<string> ApproverFilePaths { get; set; }
    }
    public class AcceptorOrApproverInfo
    {
        public DateTime SignedDateTime { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
    }
}