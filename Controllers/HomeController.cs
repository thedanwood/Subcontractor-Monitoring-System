using BQC.DataAccessLayer;
using BQC.DataAccessLayer.Dictionaries;
using BQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BQC.Controllers
{
    public class HomeController : BaseController
    {
        BQCDictionary BQCDictionary = new BQCDictionary();

        public ActionResult Index()
        {
            return View("~/Views/Sites/ChooseSite.cshtml");
        }

        public void SiteUserManual()
        {
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Build_Quality_Checklist_Manual_Site.pdf");
            Response.TransmitFile(Server.MapPath("~/Files/Build_Quality_Checklist_Manual_Site.pdf"));
            Response.End();
        }
        public void SubContractorUserManual()
        {
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Build_Quality_Checklist_Manual_SubContractor.pdf");
            Response.TransmitFile(Server.MapPath("~/Files/Build_Quality_Checklist_Manual_SubContractor.pdf"));
            Response.End();
        }
        public JsonResult GetSites()
        {
            var sitesData = BQCDictionary.GetActiveSites();

            return Json(new { data = sitesData }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ChoosePlots(int SiteID, int Role, string WorkerEmail, string SiteEmail)
        //{
        //    ChoosePlotsModel choosePlotsModel = new ChoosePlotsModel()
        //    {
        //        SiteID = SiteID,
        //        Role = Role,
        //        RoleNameEnum = BuildQualityDictionary.GetEnumDescription((Enums.EnumAllRoles)Role),
        //        WorkerEmail = WorkerEmail,
        //        SiteEmail = SiteEmail,
        //    };

        //    if(Role == (int)Enums.EnumAllRoles.SiteManagement)
        //    {
        //        return View("SiteChoosePlots",choosePlotsModel);
        //    } else
        //    {
        //        return View("WorkerChoosePlots", choosePlotsModel);
        //    }
            
        //}

        //public JsonResult GetPlotsForSite(int SiteID)
        //{
        //    var plotsData =  BQCDictionary.preparePlotsForSite(SiteID);

        //    return Json(new { data = plotsData }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetPlotsForWorker(int SiteID, int Role)
        //{
        //    var plotsData = BuildQualityDictionary.preparePlotsForWorker(SiteID, Role);

        //    return Json(new { data = plotsData }, JsonRequestBehavior.AllowGet);
        //}

        ////change in future to check if site staff or not 
        //public ActionResult CheckApproval(int SiteID)
        //{
        //    return RedirectToAction("Checks", "Checklist", new { id = SiteID });
        //}
    }
}