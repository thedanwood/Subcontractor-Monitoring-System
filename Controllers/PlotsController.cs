using BQC.DataAccessLayer.Dictionaries;
using BQC.Models;
using BQC.Models.Checklist;
using BQC.Models.Tables;
using BQC.ViewModels;
using BuildQualityChecklist.Models;
using BuildQualityChecklist.Models.Checklist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BQC.Controllers
{
    public class PlotsController : BaseController
    {
        BQCDictionary BQCDictionary = new BQCDictionary();
        public ActionResult ChoosePlots(string siteId)//encrypted
        {
            if (BQCDictionary.doesUserHaveAccessToSite(null, siteId))
            {
                return View("~/Views/Plots/ChoosePlots.cshtml", (object)siteId);
            }
            else
            {
                return View("~/Views/Sites/ChooseSite.cshtml");
            }
        }
        public JsonResult GetPlotsFromSiteID(string siteId)
        {
            List<PlotInformationModel> plotsData = new List<PlotInformationModel>();
            int decryptedSiteId = int.Parse(Utils.Decrypt(siteId));
            if (BQCDictionary.doesUserHaveAccessToSite(decryptedSiteId, null))
            {
                plotsData = BQCDictionary.getPlotsFromSiteID(decryptedSiteId);
            }
            return Json(new { data = plotsData.OrderBy(r=>r.PlotNumber).ToList() }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPlotsForPlotSelectionTable(int SiteID)
        {
            List<ChoosePlotsTableModel> plotsData = BQCDictionary.getPlotsForPlotSelectionTable(SiteID);
            return Json(new { data = plotsData }, JsonRequestBehavior.AllowGet);
        }
    }
}