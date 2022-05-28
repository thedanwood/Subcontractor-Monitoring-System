using BQC.DataAccessLayer.Dictionaries;
using BQC.Models.Checklist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BQC.Controllers
{
    public class SitesController : BaseController
    {
        BQCDictionary BQCDictionary = new BQCDictionary();
        public ActionResult ChooseSite()
        {
            return View();
        }

        public JsonResult GetSites(int? RoleEnumValue)
        {
            List<SiteInformationModel> sitesData = BQCDictionary.GetActiveSites();

            return Json(new { data = sitesData }, JsonRequestBehavior.AllowGet);
        }
    }
}