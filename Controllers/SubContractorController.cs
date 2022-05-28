using BuildQualityChecklist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildQualityChecklist.DataAccessLayer;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using BuildQualityChecklist.Models.Checklist;
using BuildQualityChecklist.DataAccessLayer.Dictionaries;

namespace BuildQualityChecklist.Controllers
{
    [Authorize]
    public class SubContractorController : Controller
    {
        BQCDictionary BQCDictionary = new BQCDictionary();
        public ActionResult LoadWorkerChecklist(string encryptedSiteID, int PlotID, int SectionID)
        {
            string decryptedSiteID = Utils.Decrypt(encryptedSiteID);

            //string[] vals = param.Split(',');


            //int siteID = Convert.ToInt32(vals[0]);
            //int role = Convert.ToInt32(vals[1]);

            ChecklistModel checklistModel = BQCDictionary.getTradeContractorChecklistModel(PlotID, SectionID);

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser u = userManager.FindByName(User.Identity.Name);
            string email = u.Email;

            ChecksModel.WorkerEmail = email;
            ChecksModel.SiteEmail = BuildQualityDictionary.getSiteEmailFromSiteID(siteID);

            //   ChecksModel.WorkerEmail = "david.fish@croudace.co.uk";
            // ChecksModel.SiteEmail = "david.fish@croudace.co.uk";

            return View("TradeContractorChecklist", ChecksModel);
        }
        // GET: SC
        //public ActionResult Index(string d)
        //{
        //    string param = Utils.Decrypt(d);

        //    string[] vals = param.Split(',');


        //    int siteID = Convert.ToInt32(vals[0]);
        //    int role = Convert.ToInt32(vals[1]);
        //    string site = "";


        //    using (CroudaceContext ctx = new CroudaceContext())
        //    {
        //        site = ctx.Sites.Where(x => x.SiteID == siteID).FirstOrDefault().Name;
        //    }

        //    ChoosePlotsModel choosePlotsModel = new ChoosePlotsModel()
        //    {
        //        SiteID = siteID,
        //        Role = role,
        //        RoleName = BuildQualityDictionary.GetEnumDescription((Enums.EnumAllRoles)role),
        //     SiteName = site,
        //        EncQuery = d
        //    };

        //    return View("WorkerChoosePlots", choosePlotsModel);
        //}
        //public ActionResult Checks(int PlotID, string d)
        //{
        //    string param = Utils.Decrypt(d);

        //    string[] vals = param.Split(',');

        //    int siteID = Convert.ToInt32(vals[0]);
        //    int role = Convert.ToInt32(vals[1]);

        //    //var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    //var userManager = new UserManager<ApplicationUser>(store);
        //    //ApplicationUser u = userManager.FindByName(User.Identity.Name);
        //    //string email = u.Email;

        //    ChooseWorkerSectionModel chooseWorkerSectionModel = new ChooseWorkerSectionModel()
        //    {
        //        Role = role,
        //        PlotID = PlotID,
        //        SiteID = siteID,
        //        EncQuery = d,
        //            RoleName = BuildQualityDictionary.GetEnumDescription((Enums.EnumAllRoles)role),
        //            PlotName = BuildQualityDictionary.getPlotNumberFromPlotID(PlotID).ToString(),
        //            SiteName = BuildQualityDictionary.getSiteNameFromPlotID(PlotID)
        //            //WorkerEmail = email
        //        };

        //        return View("ChooseWorkerSection", chooseWorkerSectionModel);



        //}
        //public ActionResult GetWorkerSections(int PlotID, int Role)
        //{
        //    List<PrepareWorkerSectionsModel> Model = BuildQualityDictionary.prepareWorkerSections(PlotID, Role);

        //    return Json(new { data = Model }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetPlotsForWorker(int SiteID, int Role)
        //{
        //    var plotsData = BuildQualityDictionary.preparePlotsForWorker(SiteID, Role);

        //    return Json(new { data = plotsData }, JsonRequestBehavior.AllowGet);
        //}


    }

}