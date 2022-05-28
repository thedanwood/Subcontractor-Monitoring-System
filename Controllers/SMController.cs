using BuildQualityChecklist.DataAccessLayer;
using BuildQualityChecklist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildQualityDomain.Models;

namespace BuildQualityChecklist.Controllers
{
    public class SMController : Controller
    {
        BuildQualityDictionary BuildQualityDictionary = new BuildQualityDictionary();
        // GET: SM
        [Authorize]
        public ActionResult Index(int s)
        {
            using (CroudaceContext ctx = new CroudaceContext())
            {
                Site site = ctx.Sites.Where(x => x.SiteID == s).FirstOrDefault();



                ChoosePlotsModel choosePlotsModel = new ChoosePlotsModel()
                {
                    SiteID = s,
                    SiteName = site.Name
                    //Role = 1,
                    //RoleName = "Site Management",
                    //WorkerEmail = "",
                    //SiteEmail = "david.fish@croudace.co.uk"
                };




                return View("SiteChoosePlots", choosePlotsModel);
            }
        }

        public JsonResult GetPlotsForSite(int SiteID)
        {
            var plotsData = BuildQualityDictionary.preparePlotsForSite(SiteID);

            return Json(new { data = plotsData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Checks(int PlotID)
        {
            SectionModel sectionModel = BuildQualityDictionary.prepareSubSections(PlotID, null);

            //sectionModel.SiteEmail = SiteEmail;
            //sectionModel.WorkerEmail = WorkerEmail;
            sectionModel.PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString();
            //return View("_SiteChecks", sectionModel.SubSections[0]);

            return View("Section", sectionModel);
        }
    }
}