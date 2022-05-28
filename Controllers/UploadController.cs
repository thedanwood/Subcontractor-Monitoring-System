using BQC.DataAccessLayer;
using BQC.DataAccessLayer.Dictionaries;
using BQC.Enums;
using SiteManagementSystemDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BQC.Controllers
{
    public class UploadController : BaseController
    {
        IdentityDictionary identityDictionary = new IdentityDictionary();
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, int? PlotID, int? SubSectionID, int QuestionID, int QuestionAttemptCountForNewFileSave, int AcceptorOrApproverEnum)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        string attemptsPath = "~/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave;
                        bool attemptsPathExists = System.IO.Directory.Exists(Server.MapPath(attemptsPath));
                        if (!attemptsPathExists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath(attemptsPath));
                        }

                        if (AcceptorOrApproverEnum == (int)AcceptorOrApprover.acceptor)
                        {
                            string fullPath = "~/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Acceptor";
                            bool fullPathExists = System.IO.Directory.Exists(Server.MapPath(fullPath));
                            if (!fullPathExists)
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(fullPath));
                            }
                        }
                        else if (AcceptorOrApproverEnum == (int)AcceptorOrApprover.approver)
                        {
                            string fullPath = "~/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Approver";
                            bool fullPathExists = System.IO.Directory.Exists(Server.MapPath(fullPath));
                            if (!fullPathExists)
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(fullPath));
                            }
                        }


                        string _FileName = Path.GetFileName(file.FileName);

                        //default to acceptor
                        string filePathForDatabase = "/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Acceptor/" + file.FileName;
                        string filePathForFileSave = Path.Combine(Server.MapPath("~/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Acceptor"), _FileName);
                        //change if approver
                        if (AcceptorOrApproverEnum == (int)AcceptorOrApprover.approver)
                        {
                            filePathForFileSave = Path.Combine(Server.MapPath("~/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Approver"), _FileName);
                            filePathForDatabase = "/UploadedFiles/PlotID" + PlotID + "/SubSectionID" + SubSectionID + "/QuestionID" + QuestionID + "/Attempt" + QuestionAttemptCountForNewFileSave + "/Approver/" + file.FileName;
                        }

                        file.SaveAs(filePathForFileSave);

                        BQCFiles bqcfile = new BQCFiles()
                        {
                            PlotID = (int)PlotID,
                            BQCQuestionID = QuestionID,
                            FileName = file.FileName,
                            FilePath = filePathForDatabase,
                            AttemptCounter = QuestionAttemptCountForNewFileSave,
                            UserID = identityDictionary.getUserIDOfCurrentUser(),
                            AcceptorOrApproverEnum = AcceptorOrApproverEnum,
                        };

                        db.BQCFiles.Add(bqcfile);
                        db.SaveChanges();
                    }

                    return Json(new { data = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { data = false }, JsonRequestBehavior.AllowGet);
                }
            }

        }
    }
}