using BQC.DataAccessLayer;
using BuildQualityDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildQualityChecklist.Controllers
{
    public class AdminController : Controller
    {
        public class UpdateQuestionViewModel
        {
            public int? QuestionID { get; set; }
            public string QuestionText { get; set; }
            public int UpdateType { get; set; }
        }
        public ActionResult renderUpdateQuestionPartialView(int? Id, int UpdateType)
        {
            using(SiteManagementSystemContext smsContext = new SiteManagementSystemContext())
            {
                string questionText = smsContext.BQCQuestions.Where(r => r.BQCQuestionID == Id).Select(r => r.QuestionText).FirstOrDefault();
                UpdateQuestionViewModel viewModel = new UpdateQuestionViewModel()
                {
                    UpdateType = UpdateType,
                    QuestionID = Id,
                    QuestionText = questionText,
                };
                return PartialView("~/Views/Admin/_UpdateChecklistQuestion.cshtml", viewModel);
            }
        }
        public ActionResult EditChecklistQuestions()
        {
            return View();
        }
        public JsonResult deleteChecklistQuestion(int Id)
        {
            using(SiteManagementSystemContext smsContext = new SiteManagementSystemContext())
            {
                BQCQuestions question = smsContext.BQCQuestions.Where(r=>r.BQCQuestionID == Id).FirstOrDefault();
                question.Active = false;
                smsContext.SaveChanges();
                 
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getEditQuestionsInSubSection(int subSectionID)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                var data = db.BQCQuestions.Where(r => r.BQCSubSectionID == subSectionID).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet); 
            }
        }
    }
}