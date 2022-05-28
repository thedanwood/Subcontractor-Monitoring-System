using BuildQualityChecklist.DataAccessLayer;
using BuildQualityChecklist.Models;
using BuildQualityChecklist.Models.Checklist;
using BuildQualityChecklist.ViewModels;
using BuildQualityDomain.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;

namespace BuildQualityChecklist.Controllers
{
    public class ChecklistControllerOld : Controller
    {
        BuildQualityDictionary BuildQualityDictionary = new BuildQualityDictionary();
        // GET: Checklist
        //public ActionResult ChooseRole(int SiteID)
        //{
        //    //List<string> WorkerRoles = new BuildQualityDictionary.DescriptionAttributes<Enums.WorkerRoles>().Descriptions.ToList();

        //    return View(SiteID);
        //}

        public ActionResult Checks(int PlotID)
        {
            if (User.IsInRole("Croudace Site Staff"))
            {
                SectionModel sectionModel = BuildQualityDictionary.prepareSubSections(PlotID, null);

                using (CroudaceContext ctx = new CroudaceContext())
                {
                    SitePlot sp = ctx.SitePlots.Where(x => x.PlotID == PlotID).FirstOrDefault();
                    Site site = ctx.Sites.Where(x => x.SiteID == sp.SiteID).FirstOrDefault();
                    

                    sectionModel.HomeURL = "http://localhost/buildqualitychecklist/sm?s=" + sp.SiteID;
                    //sectionModel.SiteEmail = SiteEmail;
                    //sectionModel.WorkerEmail = WorkerEmail;
                    sectionModel.PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString();
                    //return View("_SiteChecks", sectionModel.SubSections[0]);

                    return View("Section", sectionModel);
                }

            }
            else
            {
                //ChooseWorkerSectionModel chooseWorkerSectionModel = new ChooseWorkerSectionModel()
                //{
                //    Role = Role,
                //    PlotID = PlotID,
                //    RoleName = BuildQualityDictionary.GetEnumDescription((Enums.EnumAllRoles)Role),
                //    PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString(),
                //    SiteName = BuildQualityDictionary.getSiteNameFromPlotID(PlotID),
                //    WorkerEmail = WorkerEmail,
                //    SiteEmail = SiteEmail,
                //};

                // return View("ChooseWorkerSection", chooseWorkerSectionModel);
                return null;
            }
            //worker
            //if (Role != (int)Enums.EnumAllRoles.SiteManagement)
            //{
            //    ChooseWorkerSectionModel chooseWorkerSectionModel = new ChooseWorkerSectionModel()
            //    {
            //        Role = Role,
            //        PlotID = PlotID,
            //        RoleName = BuildQualityDictionary.GetEnumDescription((Enums.EnumAllRoles)Role),
            //        PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString(),
            //        SiteName = BuildQualityDictionary.getSiteNameFromPlotID(PlotID),
            //        WorkerEmail = WorkerEmail,
            //        SiteEmail = SiteEmail,
            //    };

            //    return View("ChooseWorkerSection", chooseWorkerSectionModel);
            //}
            ////site management
            //else
            //{
            //    SectionModel sectionModel = BuildQualityDictionary.prepareSubSections(PlotID, null);
                
            //    sectionModel.HomeURL = "http://localhost/buildqualitychecklist/sm?s=";
            //    sectionModel.SiteEmail = SiteEmail;
            //    sectionModel.WorkerEmail = WorkerEmail;
            //    sectionModel.PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString();
            //    //return View("_SiteChecks", sectionModel.SubSections[0]);

            //    return View("Section",sectionModel);
            //}
        }
        public ActionResult getSubSectionPartialView(int SectionID, int SubSectionID, int PlotID)
        {
            ChecklistModel checklistModel = BuildQualityDictionary.getSubSectionPartialView(SectionID, SubSectionID, PlotID);

            //test only
            checksModel.SiteEmail = renderPartialModel.SiteEmail;
            checksModel.WorkerEmail = renderPartialModel.WorkerEmail;

            var partialView = PartialView();

            if (checksModel.PaymentAuthModel!=null)
            {
                return PartialView("_PaymentAuth", checksModel);
            } 
            else if (checksModel.SubSectionModel.RoleEnumValue == (int)Enums.EnumAllRoles.SiteManagement)
            {
                return PartialView("_SiteChecks", checksModel);
            } else
            {
                return PartialView("_WorkerSiteApproval", checksModel);
            }
        }

        public ActionResult loadSectionSelected(int Section, int PlotID)
        {
            SectionAndSubSection newSectionAndSubSection = BuildQualityDictionary.loadSectionSelected(PlotID, Section);

            SectionModel sectionModel = BuildQualityDictionary.prepareSubSections(PlotID, newSectionAndSubSection.Section);
            sectionModel.PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString();
            sectionModel.HomeURL = "http://localhost/buildqualitychecklist/sm?s=";
            // sectionModel.SiteEmail = SiteEmail;
            //  sectionModel.WorkerEmail = WorkerEmail;

            return View("Section", sectionModel);
        }

        public ActionResult GetWorkerSections(int PlotID, int Role)
        {
            List<PrepareWorkerSectionsModel> Model = BuildQualityDictionary.prepareWorkerSections(PlotID, Role);

            return Json(new { data = Model }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrepareWorkerChecks (int PlotID, int SubSection, int Role, string WorkerEmail, string SiteEmail)
        {
            ChecksModel ChecksModel = BuildQualityDictionary.prepareWorkerChecks(PlotID, SubSection, Role);

            ChecksModel.WorkerEmail = WorkerEmail;
            ChecksModel.SiteEmail = SiteEmail;

            return View("WorkerChecks", ChecksModel);
        }
        public ActionResult WorkerChecks(int SubSectionID, int PlotID, int Role)
        {
            ChecksModel ChecksModel = BuildQualityDictionary.prepareWorkerChecks(PlotID, Role, SubSectionID);

            return View(ChecksModel);
        }


        public JsonResult SaveSiteChecks(SaveSubSectionModel saveSubSectionModel)
        {
            BuildQualityDictionary.saveChecks(saveSubSectionModel);

            BuildQualityDictionary.sendSiteEmail(saveSubSectionModel);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveWorkerChecklistResponses(SaveSubSectionModel saveSubSectionModel)
        {
            
            BuildQualityDictionary.saveChecks(saveSubSectionModel);

            BuildQualityDictionary.sendWorkerEmail(saveSubSectionModel);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveWorkerApprovalChecks(SaveSubSectionModel saveSubSectionModel)
        {
            BuildQualityDictionary.saveWorkerApprovalChecks(saveSubSectionModel);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePaymentAuth(SaveSubSectionModel saveSubSectionModel)
        {
            BuildQualityDictionary.savePaymentAuth(saveSubSectionModel);

          //??  BuildQualityDictionary.sendWorkerSiteApprovalEmail(saveSubSectionModel, true);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Signature()
        {
            return View();
        }
    }
}