using BQC.DataAccessLayer.Dictionaries;
using BQC.Models.Checklist;
using CroudaceDomainLibrary.Concrete;
using CroudaceDomainLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.DataAccessLayer.Dictionaries
{
    public class SiteInformationDictionary
    {
        IdentityDictionary identityDictionary = new IdentityDictionary();
        public List<SiteInformationModel> GetAllAllocatedSitesForCurrentUser()
        {
            List<SiteInformationModel> allocatedSites = new List<SiteInformationModel>();
            int? contactId = identityDictionary.getContactIDOfCurrentUser();
            if (contactId!=null)
            {
                allocatedSites = GetAllSiteContactsByContactID((int)contactId);
            }
            return allocatedSites;
        }
        public List<SiteInformationModel> GetAllSiteContactsByContactID(int ContactID)
        {
            using(CroudaceContext croudaceDb = new CroudaceContext())
            {
                List<SiteInformationModel> allocatedSites = new List<SiteInformationModel>();
                List<Site> Sites = croudaceDb.Sites.Where(m => m.Status == 1 || m.Status == 3).ToList();
                List<int?> allocatedSiteIds = croudaceDb.SiteContacts.Where(m => m.ContactID == ContactID).OrderByDescending(m => m.SiteContactID).Select(r=>r.SiteID).ToList();
                foreach (int siteId in allocatedSiteIds)
                {
                    Site validSite = croudaceDb.Sites.Where(r => r.SiteID == siteId && (r.Status == 1 || r.Status == 3)).FirstOrDefault();
                    if (validSite!=null)
                    {
                        SiteInformationModel siteInformationModel = new SiteInformationModel()
                        {
                            SiteID = validSite.SiteID,
                            MarketingName = validSite.MarketingName,
                            SiteName = validSite.Name,
                        };
                        allocatedSites.Add(siteInformationModel);
                    }
                }
                return allocatedSites;
            }
        }
    }
}