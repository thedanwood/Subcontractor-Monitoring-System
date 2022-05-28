using BQC.Enums;
using EnumsNET;
using SiteManagementSystemDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using BQC.Models.Checklist;
using BQC.Models.Tables;
using CroudaceDomainLibrary.Concrete;
using CroudaceDomainLibrary.Entities;
using BQC.Models.Email;
using BQC.Models;
using BQC.Enums;
using BuildQualityChecklist.Models;
using BuildQualityChecklist.Enums;
using System.IO;
using System.Data.SqlClient;
using BuildQualityChecklist.Models.Checklist;

namespace BQC.DataAccessLayer.Dictionaries
{
    public class BQCDictionary 
    {
        string currentUserID;
        int? currentUserRoleEnumValue;
        private string filePathPrefix = "/BuildQualityChecklist";
        IdentityDictionary identityDictionary = new IdentityDictionary();
        public BQCDictionary()
        {
            currentUserRoleEnumValue = identityDictionary.getRoleEnumValueOfCurrentUser();
            currentUserID = identityDictionary.getUserIDOfCurrentUser();
        }


        #region Sites And Plots
        public bool doesUserHaveAccessToSite(int? decryptedSiteId, string encryptedSiteId)
        {
            if (encryptedSiteId != null)
            {
                decryptedSiteId = int.Parse(Utils.Decrypt(encryptedSiteId.ToString()));
            }
            var allocatedSites = GetActiveSites();
            return (allocatedSites.Any(r => r.SiteID == decryptedSiteId));
        }
        public int getSiteIDFromPlotID(int PlotID)
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                return (int)db.SitePlots.Where(r => r.PlotID == PlotID).Select(r => r.SiteID).FirstOrDefault();
            }
        }
        public string getSiteNameFromSiteID(int SiteID)
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                return db.Sites.Where(r => r.SiteID == SiteID).Select(r => r.Name).FirstOrDefault();
            }
        }
        public string getPlotNameFromPlotID(int PlotID)
        {
            using (CroudaceContext croudacedb = new CroudaceContext())
            {
                var plot = croudacedb.SitePlots.Where(r => r.PlotID == (int)PlotID).FirstOrDefault();

                return plot.AddressLine1 + ", " + plot.AddressLine2;
            }
        }

        public List<ChoosePlotsTableModel> getPlotsForPlotSelectionTable(int SiteID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<ChoosePlotsTableModel> choosePlotsTableList = new List<ChoosePlotsTableModel>();
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    choosePlotsTableList = db.Database.SqlQuery<ChoosePlotsTableModel>("GetPlotInformationForSite_Worker @SiteID, @UserID"
                            , new SqlParameter("@SiteID", SiteID)
                            , new SqlParameter("@UserID", currentUserRoleEnumValue)
                            ).ToList();                        
                } 
                else
                {
                    choosePlotsTableList = db.Database.SqlQuery<ChoosePlotsTableModel>("GetPlotInformationForSite_Staff @SiteID"
                        , new SqlParameter("@SiteID", SiteID)
                        ).ToList();
                }

                foreach (ChoosePlotsTableModel model in choosePlotsTableList)
                {
                    model.EncryptedPlotID = Utils.Encrypt(model.PlotID.ToString());
                }
                return choosePlotsTableList;
            }            
        }
        public List<PlotInformationModel> getPlotsFromSiteID(int SiteID)
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                List<PlotInformationModel> plotInformationModel = new List<PlotInformationModel>();
                var plotsInSite = db.SitePlots.Where(r => r.SiteID == SiteID).ToList();
                foreach(var plot in plotsInSite)
                {
                    plotInformationModel.Add(new PlotInformationModel()
                    {
                        EncryptedPlotID = Utils.Encrypt(plot.PlotID.ToString()),
                        PlotNumber = (int)plot.PlotNo
                    });
                }
                return plotInformationModel;
            }
        }
        public List<SiteInformationModel> GetActiveSites()
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                List<SiteInformationModel> listSiteInfo = new List<SiteInformationModel>();
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    using (CroudaceExtranetContext croudaceExtranetDb = new CroudaceExtranetContext())
                    {
                        string currentUserID = identityDictionary.getUserIDOfCurrentUser();
                        List<int> siteIDsForTradeContractor = croudaceExtranetDb.ThirdPartySiteAccess.Where(r => r.UserId == currentUserID).Select(r => r.SiteId).ToList();
                        foreach (int siteID in siteIDsForTradeContractor)
                        {
                            string encryptedSiteId = Utils.Encrypt(siteID.ToString());
                            SiteInformationModel siteInfo = db.Sites.Where(o=>o.SiteID == siteID).Select(o => new SiteInformationModel() { SiteID = o.SiteID, SiteName = o.Name, MarketingName = o.MarketingName, EncryptedSiteID = encryptedSiteId}).FirstOrDefault();
                            listSiteInfo.Add(siteInfo);
                        }
                    }
                }
                else //staff
                {
                    int? contactId = identityDictionary.getContactIDOfCurrentUser();
                    if (contactId != null)
                    {
                        listSiteInfo = GetAllSiteContactsByContactID((int)contactId);
                    }
                }
                return listSiteInfo;
            }
        }
        public List<SiteInformationModel> GetAllSites()
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                List<SiteInformationModel> allSites = db.Sites.Where(r => r.Status == 1 || r.Status == 3).Select(r=> new SiteInformationModel() { SiteID = r.SiteID, MarketingName = r.MarketingName, SiteName = r.Name}).ToList();
                //cannot encrypt inside linq
                foreach(SiteInformationModel siteInfo in allSites)
                {
                    string encryptedSiteId = Utils.Encrypt(siteInfo.SiteID.ToString());
                    siteInfo.EncryptedSiteID = encryptedSiteId;
                }
                return allSites;
            }
        }
        public List<SiteInformationModel> GetAllSiteContactsByContactID(int ContactID)
        {
            using (CroudaceContext croudaceDb = new CroudaceContext())
            {
                List<SiteInformationModel> allocatedSites = new List<SiteInformationModel>();
                List<int?> allocatedSiteIds = new List<int?>();
                if (HttpContext.Current.User.Identity.Name == "burgessi")
                {
                    allocatedSiteIds = croudaceDb.Sites.Where(r => r.SalesRegionID == 1 && r.Status == 1 || r.Status == 3).Select(r => (int?)r.SiteID).ToList();
                }
                else if (HttpContext.Current.User.Identity.Name == "chapmanl")
                {
                    allocatedSiteIds = croudaceDb.Sites.Where(r => r.SalesRegionID == 2 && r.Status == 1 || r.Status == 3).Select(r => (int?)r.SiteID).ToList();
                }
                else
                {
                    allocatedSiteIds = croudaceDb.SiteContacts.Where(m => m.ContactID == ContactID).OrderByDescending(m => m.SiteContactID).Select(r => r.SiteID).ToList();
                }
                foreach (int siteId in allocatedSiteIds)
                {
                    Site validSite = croudaceDb.Sites.Where(r => r.SiteID == siteId /*&& (r.Status == 1 || r.Status == 3)*/).FirstOrDefault();
                    if (validSite != null)
                    {
                        SiteInformationModel siteInformationModel = new SiteInformationModel()
                        {
                            SiteID = validSite.SiteID,
                            MarketingName = validSite.MarketingName,
                            SiteName = validSite.Name,
                            EncryptedSiteID = Utils.Encrypt(validSite.SiteID.ToString()),
                        };
                        allocatedSites.Add(siteInformationModel);
                    }
                }
                return allocatedSites;
            }
        }
        public List<string> getSiteOfficeEmailInSite(int SiteID)
        {
            //local
            //List<string> emails = new List<string>();
            //emails.Add("daniel.wood@croudace.co.uk");
            //emails.Add("danielwood1412@gmail.com");
            //return emails;
            //live
            int siteOfficeEmailSiteNumberTypeID = 12;
            using (CroudaceContext db = new CroudaceContext())
            {
                return db.SiteNumbers.Where(r => r.SiteID == SiteID && r.SiteNumberID == siteOfficeEmailSiteNumberTypeID).Select(r => r.SiteNumber1).ToList();
            }
        }
        public List<string> getTradeContractorEmailsInSiteAndRole(int SiteID, int RoleEnumValue)
        {
            using(CroudaceExtranetContext db = new CroudaceExtranetContext())
            {
                List<string> allTradeContractorEmailAddressessInSiteAndTrade = new List<string>();
                //live
                List<string> allUserIDsInSite = db.ThirdPartySiteAccess.Where(r => r.SiteId == SiteID).Select(r => r.UserId).ToList();
                foreach (string userID in allUserIDsInSite)
                {
                    if (db.ThirdPartyUsers.Any(r => r.TradeTypeId == RoleEnumValue && r.UserId == userID))
                    {
                        string emailAddress = db.AspNetUsers.Where(r => r.Id == userID).Select(r => r.Email).FirstOrDefault();
                        allTradeContractorEmailAddressessInSiteAndTrade.Add(emailAddress);
                    }
                }
                //local
                //allTradeContractorEmailAddressessInSiteAndTrade.Add("daniel.wood@croudace.co.uk");
                //allTradeContractorEmailAddressessInSiteAndTrade.Add("danielwood1412@gmail.com");

                return allTradeContractorEmailAddressessInSiteAndTrade;
            }
        }
        public int? getNumberOfStoriesForPlot(int PlotID)
        {
            using (CroudaceContext croudaceContext = new CroudaceContext())
            {
                using (HouseTypesContext houseTypesContext = new HouseTypesContext())
                {
                    int? houseTypeId = croudaceContext.SitePlots.Where(r => r.PlotID == PlotID).Select(r => r.HouseTypeID).FirstOrDefault();
                    if (houseTypeId != null)
                    {
                        int? baseTypeId = houseTypesContext.PlotHouseTypes.Where(r => r.HouseTypeID == houseTypeId).Select(r => r.BaseTypeID).FirstOrDefault();
                        if (baseTypeId != null)
                        {
                            int? numberOfStoreys = houseTypesContext.SiteBaseHouseTypes.Where(r => r.BaseTypeID == baseTypeId).Select(r => r.Storeys).FirstOrDefault();
                            return numberOfStoreys;
                        }
                    }
                    return null;
                }
            }
        }
        public PlotInformationModel getPlotInformationModel(int plotId)
        {
            return new PlotInformationModel() {
                PlotID = plotId,
                PlotName = getPlotNameFromPlotID(plotId),
                EncryptedPlotID = Utils.Encrypt(plotId.ToString()),
            };
        }
        public SiteInformationModel getSiteInformationModel(int siteId)
        {
            return new SiteInformationModel()
            {
                SiteID = siteId,
                SiteName = getSiteNameFromSiteID(siteId),
                EncryptedSiteID = Utils.Encrypt(siteId.ToString()),
            };
        }
        #endregion Sites And Plots



        #region Tables
        public List<TradeContractorSectionsTableModel> getTradeContractorSections(int PlotID)
        {
            List<TradeContractorSectionsTableModel> listSectionsModel = new List<TradeContractorSectionsTableModel>();

            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<BQCSubSections> allSubSectionsForRole = db.BQCSubSections.Where(r => r.RoleNameEnum == currentUserRoleEnumValue && r.SubSectionTypeEnum == (int)SubSectionType.TradeContractorChecklist).ToList();

                foreach (BQCSubSections SubSection in allSubSectionsForRole)
                {
                    int status = getStatusOfTradeContractorSubSection(SubSection.BQCSubSectionID, PlotID);
                    TradeContractorSectionsTableModel sectionsModel = new TradeContractorSectionsTableModel()
                    {
                        SectionID = SubSection.BQCSectionID,
                        FullSectionNameString = getSectionFullNameFromSectionID(SubSection.BQCSectionID),
                        //TotalBuildSectionsCompleted = getTotalBuildSectionsCompletedAndTotalInPlot(PlotID),
                        Status = status,
                    };
                    listSectionsModel.Add(sectionsModel);
                }
            }

            return listSectionsModel;
        }
        #endregion Tables



        #region Section Naming Info
        public List<BQCSections> getAllSectionsList()
        {
            using(SiteManagementSystemContext smsContext = new SiteManagementSystemContext())
            {
                return smsContext.BQCSections.ToList();
            }
        }
        public List<SectionNamingInformationModel> getAllSectionNamingInfoList()
        {
            using (SiteManagementSystemContext smsContext = new SiteManagementSystemContext())
            {
                List<SectionNamingInformationModel> sectionNamingInfoList = new List<SectionNamingInformationModel>();
                List<BQCSections> sectionsList = smsContext.BQCSections.ToList();
                foreach(BQCSections section in sectionsList)
                {
                    SectionNamingInformationModel sectionNamingInfo = new SectionNamingInformationModel()
                    {
                        SectionFullName = getSectionFullNameFromSectionID(section.BQCSectionID),
                        SectionID = section.BQCSectionID,
                    };
                    sectionNamingInfoList.Add(sectionNamingInfo);
                }
                return sectionNamingInfoList;
            }
        }

        public List<BQCSections> getSectionsListForPlot(int PlotID)
        {
            using(SiteManagementSystemContext smsContext = new SiteManagementSystemContext())
            {
                int? numberOfStories = getNumberOfStoriesForPlot(PlotID);

                List<BQCSections> allApplicableSections = getAllSectionsList();
                if(numberOfStories!=null) 
                { 
                    allApplicableSections = allApplicableSections.Where(r => r.ApplicableToStoreysGreaterThanOrEqualTo <= numberOfStories).ToList(); 
                }
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    allApplicableSections = smsContext.BQCSections.Where(r => r.RoleEnum == currentUserRoleEnumValue).ToList();
                }
                return allApplicableSections.OrderBy(r=>r.SortOrderValue).ToList();
            }
            
        } 
        public bool isSectionApplicableToPlot(int SectionID, int PlotID)
        {
            List<BQCSections> bqcSections = getSectionsListForPlot(PlotID);
            return (bqcSections.Any(r => r.BQCSectionID == SectionID));
        }
        public List<SectionNamingInformationModel> getListSectionDetails(int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<BQCSections> allApplicableSections = getSectionsListForPlot(PlotID);

                List<SectionNamingInformationModel> allSectionDetails = new List<SectionNamingInformationModel>();

                foreach (BQCSections section in allApplicableSections)
                {
                    SectionNamingInformationModel sectionDetails = getDetailsOfSection(section);

                    allSectionDetails.Add(sectionDetails);
                }
                return allSectionDetails;
            }
        }
        public SectionNamingInformationModel getDetailsOfSection(BQCSections Section)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                SectionNamingInformationModel sectionDetails = new SectionNamingInformationModel()
                {
                    SectionNumber = Section.BQCSectionID,
                    SectionFullName = getSectionFullNameFromSectionID(Section.BQCSectionID),
                };

                return sectionDetails;
            }
        }
        public string getSectionFullNameFromSectionID(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                SubSectionAndSectionNamingEnumValuesModels subSectionAndSectionNamingEnumValue = getSubSectionAndSectionNamingEnumValues(SectionID);

                return getSectionFullNameFromEnumValues(subSectionAndSectionNamingEnumValue);
            }
        }
        public SubSectionAndSectionNamingEnumValuesModels getSubSectionAndSectionNamingEnumValues(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //try get subsection category enum value from worker subsection but if there isnt a worker subsection then get from any subsection thats not payment auth
                var subSectionCategoryEnum = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.TradeContractorChecklist).Select(r => (int?)r.SubSectionCategoryNameEnum).DefaultIfEmpty(null).FirstOrDefault() ??
                    db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.StaffChecklist).Select(r => (int?)r.SubSectionCategoryNameEnum).DefaultIfEmpty(null).FirstOrDefault();

                var SectionCategoryEnum = db.BQCSections.Where(r => r.BQCSectionID == SectionID).Select(r => r.SectionCategoryEnum).FirstOrDefault();

                //try get subsection role enum value from worker subsection but if there isnt a worker subsection then get from any subsection thats not payment auth
                int? subSectionRoleEnumValue = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.TradeContractorChecklist).Select(r => (int?)r.RoleNameEnum).DefaultIfEmpty(null).FirstOrDefault() ??
                    db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.StaffChecklist).Select(r => r.RoleNameEnum).FirstOrDefault();

                return new SubSectionAndSectionNamingEnumValuesModels()
                {
                    SubSectionCategoryEnum = (int)subSectionCategoryEnum,
                    SectionCategoryEnum = (int)SectionCategoryEnum,
                    SubSectionRoleEnumValue = subSectionRoleEnumValue,
                };
            }
        }
        public string getSectionFullNameFromEnumValues(SubSectionAndSectionNamingEnumValuesModels SubSectionAndSectionNamingEnumValue)
        {
            string subSectionCategoryName = ((SubSectionCategoryNames)SubSectionAndSectionNamingEnumValue.SubSectionCategoryEnum).AsString(EnumFormat.Description);
            string sectionCategoryName = ((SectionCategoryNames)SubSectionAndSectionNamingEnumValue.SectionCategoryEnum).AsString(EnumFormat.Description);
            string subSectionRoleNameEnum = ((AllRoles)SubSectionAndSectionNamingEnumValue.SubSectionRoleEnumValue).AsString(EnumFormat.Description);
            string fullSectionName = sectionCategoryName;

            if (subSectionCategoryName != null)
                if (sectionCategoryName != subSectionCategoryName)
                    fullSectionName += " - " + subSectionCategoryName;
            if (subSectionRoleNameEnum != null)
                fullSectionName += " (" + subSectionRoleNameEnum + ")";

            return fullSectionName;
        }
        #endregion Section Naming Info



        #region Checklist
        //build stages refers to build plan, build sections refers to bqc
        private int totalNumberOfBuildStages = 32;
        public int getSubSectionTypeEnum(BQCSubSections SubSection, int? subsectionId)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                if (subsectionId != null && SubSection == null)
                {
                    SubSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == subsectionId).FirstOrDefault();
                }
                if (isSubSectionAPaymentAuthSubSection(SubSection))
                {
                    return (int)SubSectionType.PaymentAuthorisation;
                }
                else if (isSubSectionATradeContractorSubSection(SubSection))
                {
                    return (int)SubSectionType.TradeContractorChecklist;
                }
                else if (isSubSectionAStaffSubSection(SubSection))
                {
                    return (int)SubSectionType.StaffChecklist;
                }
                return 0;
            }
        }
        public bool isSubSectionAPaymentAuthSubSection(BQCSubSections SubSection)
        {
            return SubSection.SubSectionTypeEnum == (int)SubSectionType.PaymentAuthorisation;
        }
        public bool isSubSectionATradeContractorSubSection(BQCSubSections SubSection)
        {
            return SubSection.SubSectionTypeEnum == (int)SubSectionType.TradeContractorChecklist;
        }
        public bool isSubSectionAStaffSubSection(BQCSubSections SubSection)
        {
            return SubSection.SubSectionTypeEnum == (int)SubSectionType.StaffChecklist;
        }
        public int getStatusOfSubSection(int SubSectionID, int PlotID)
        {
            //rolenumvalue is redundant for this code
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                BQCSubSections subSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).FirstOrDefault();

                int subSectionTypeEnumValue = getSubSectionTypeEnum(subSection, null);

                if (subSectionTypeEnumValue == (int)SubSectionType.PaymentAuthorisation) // payment auth subsection
                {
                    return getStatusOfPaymentAuthSubSection(SubSectionID, PlotID);
                }
                else if (subSectionTypeEnumValue == (int)SubSectionType.StaffChecklist)
                {
                    return getStatusOfStaffSubSection(SubSectionID, PlotID);
                } 
                else if (subSectionTypeEnumValue == (int)SubSectionType.TradeContractorChecklist)
                {
                    return getStatusOfTradeContractorSubSection(SubSectionID, PlotID);
                }
                return 0;
            }
        }
        public int getStatusOfPaymentAuthSubSection(int SubSectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                bool isComplete = db.BQCSubSectionSignatures.Any(r => r.PlotID == PlotID && r.BQCSubSectionID == SubSectionID);
                if (isComplete)
                {
                    return (int)StatusOfPaymentAuthorisationSubSection.PaymentAuthorised;
                } else
                {
                    return (int)StatusOfPaymentAuthorisationSubSection.PaymentNotYetAuthorised;
                }
            }
        }
        public bool isPaymentAuthorisedForSubSection(int SubSectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int sectionID = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.BQCSectionID).FirstOrDefault();

                return isPaymentAuthorisedForSection(sectionID, PlotID);
            }
        }
        public int getStatusOfTradeContractorSubSection(int SubSectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int status = 0;
                var model = getSubSectionQuestionsResponseInformation(PlotID, SubSectionID);
                bool isSigned = db.BQCSubSectionSignatures.Any(r => r.BQCSubSectionID == SubSectionID && r.PlotID == PlotID);
                bool isPaymentSigned = isPaymentAuthorisedForSubSection(SubSectionID, PlotID);
                bool subsectionIsApproved = (model.TotalQuestionsApproved + model.TotalQuestionsMarkedAsConditional == model.TotalQuestionsInSubSection);

                int sectionID = getSectionIDFromSubSectionID(SubSectionID);
                if (!checkDependentSectionsAndSubSectionsAreCompleted(sectionID, PlotID))
                {
                    return (int)StatusOfTradeContractorSubSection.SubSectionDependenciesIncompleted;
                }
                if (!isSigned && model.TotalQuestionsAttempted != model.TotalQuestionsInSubSection)
                {
                    status = (int)StatusOfTradeContractorSubSection.IncompleteByTradeContractor;
                }
                if (isSigned && model.TotalQuestionsAttempted == model.TotalQuestionsInSubSection && model.TotalQuestionsResponded != model.TotalQuestionsInSubSection)
                {
                    status = (int)StatusOfTradeContractorSubSection.AwaitingResponseFromStaff;
                }
                else if (isSigned && model.TotalQuestionsAttempted == model.TotalQuestionsInSubSection && model.TotalQuestionsResponded == model.TotalQuestionsInSubSection && !subsectionIsApproved)
                {
                    status = (int)StatusOfTradeContractorSubSection.AmendmentByTradeContractorNeeded;
                }
                else if (isSigned && subsectionIsApproved && !isPaymentSigned)
                {
                    status = (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised;
                }
                else if (isSigned && subsectionIsApproved && isPaymentSigned)
                {
                    status = (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised;
                }

                return status;
            }
        }
        public bool checkDependentSectionsAndSubSectionsAreCompleted(int SectionID, int PlotID)
        {
            return (checkDependentSubSectionIsCompleted(SectionID, PlotID) /*& checkDependentSectionsAreCompleted(SectionID, PlotID)*/);
        }
        public bool checkDependentSubSectionIsCompleted(int sectionID, int plotID)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                // check if there is site subsection in section and if completed
                int? siteSubSectionID = getSiteSubSectionIDFromSectionID(sectionID);
                if (siteSubSectionID != null)
                {
                    int staffSubSectionStatus = getStatusOfStaffSubSection((int)siteSubSectionID, plotID);
                    if (HttpContext.Current.User.IsInRole("Subcontractor"))
                    {
                        return (staffSubSectionStatus == (int)StatusOfStaffSubSection.Complete);
                    }
                }
                return true;
                // else check if final subsection in prior section is completed
            }


        }
        public bool checkDependentSectionsAreCompleted(int sectionId, int plotId)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<BQCSectionDependencies> allSectionDependencies = db.BQCSectionDependencies.Where(r => r.BQCSectionId == sectionId).ToList();
                
                int? plotSectionStartingPoint = getPlotSectionStartingPoint(plotId);
                int plotSectionStartingPointSortOrderValue = 0;
                if (plotSectionStartingPoint != null)
                {
                    plotSectionStartingPointSortOrderValue = db.BQCSections.Where(r => r.BQCSectionID == (int)plotSectionStartingPoint).Select(r => r.SortOrderValue).FirstOrDefault();
                }

                bool andDependenciesValid = true;
                bool orDependenciesValid = false;
                if(allSectionDependencies.Count() == 0) // no dependecy records means can be completed any time
                {
                    andDependenciesValid = orDependenciesValid = true;
                }
                if(!allSectionDependencies.Any(r=>r.DependencyTypeEnum == (int)DependencyTypes.ReliantOnCompletionOrOtherCompletion))
                {
                    orDependenciesValid = true;
                }
                foreach (BQCSectionDependencies dependency in allSectionDependencies)
                {
                    int sortOrderValueOfDependentSection = db.BQCSections.Where(r => r.BQCSectionID == dependency.DependentOnSectionId).Select(r => r.SortOrderValue).FirstOrDefault();
                    bool sectionIsCompleted = isSectionCompleted(dependency.DependentOnSectionId, plotId);
                    if (dependency.DependencyTypeEnum == (int)DependencyTypes.ReliantOnCompletion)
                    {
                        if (!sectionIsCompleted && sortOrderValueOfDependentSection > plotSectionStartingPointSortOrderValue)
                        {
                            andDependenciesValid = false;
                        }
                    }
                    if (dependency.DependencyTypeEnum == (int)DependencyTypes.ReliantOnCompletionOrOtherCompletion)
                    {
                        if (sectionIsCompleted || sortOrderValueOfDependentSection < plotSectionStartingPointSortOrderValue)
                        {
                            orDependenciesValid = true;
                        }
                    }
                }
                return (andDependenciesValid && orDependenciesValid);
            }
            
        }
        public bool isPaymentAuthorisedForSection(int SectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                var paymentAuthSubSectionID = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.PaymentAuthorisation).Select(r => r.BQCSubSectionID).FirstOrDefault();
                return db.BQCSubSectionSignatures.Any(r => r.PlotID == PlotID && r.BQCSubSectionID == paymentAuthSubSectionID);
            }
        }
        public ChecklistModel getSubSectionPartialView(int SubSectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                BQCSubSections subSection = db.BQCSubSections.FirstOrDefault(r => r.BQCSubSectionID == SubSectionID);
                
                SiteAndPlotInformationModel sitePlotInfo = new SiteAndPlotInformationModel();
                int siteID = getSiteIDFromPlotID(PlotID);
                sitePlotInfo.SiteInfo = new SiteInformationModel() { SiteID = siteID, SiteName = getSiteNameFromSiteID(siteID) };
                sitePlotInfo.PlotInfo = new PlotInformationModel() { PlotID = PlotID, PlotName = getPlotNameFromPlotID(PlotID) };

                ChecklistModel checklistModel = new ChecklistModel()
                {
                    SiteAndPlotInfo = sitePlotInfo,
                    PaymentAuthModel = new PaymentAuthorisationModel(),
                };

                int subSectionType = getSubSectionTypeEnum(subSection, null);

                if (subSectionType == (int)SubSectionType.PaymentAuthorisation)
                {
                    checklistModel.PaymentAuthModel = getPaymentAuthorisationModel(subSection, PlotID);
                }
                if (subSectionType == (int)SubSectionType.TradeContractorChecklist)
                {
                    checklistModel.SubSectionModel = getTradeContractorChecklistStaffApprovalModel(subSection, PlotID);
                }
                if (subSectionType == (int)SubSectionType.StaffChecklist)
                {
                    checklistModel.SubSectionModel = getStaffChecklistModel(subSection, PlotID);
                }

                if (subSectionType == (int)SubSectionType.TradeContractorChecklist || subSectionType == (int)SubSectionType.StaffChecklist)
                {
                    checklistModel = getChecklistIndexValues(checklistModel);
                }

                return checklistModel;
            }
        }
        public ChecklistModel getChecklistIndexValues(ChecklistModel checklistModel)
        {
            for (int i = 0; i < checklistModel.SubSectionModel.ListQuestionAndResponseInfo.Count(); i++)
            {
                var mostRecentAttemptIndexValue = (checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo.Count() > 0) ? checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo.Count() - 1 : 0;
                var mostRecentAttachedAcceptorFilePathIndexValue = 0;
                var mostRecentAttachedApproverFilePathIndexValue = 0;
                QuestionAttemptResponseInfo mostRecentAttemptResponseInfo = new QuestionAttemptResponseInfo();
                if (checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo.Count() > 0)
                {
                    mostRecentAttachedAcceptorFilePathIndexValue = (checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo[mostRecentAttemptIndexValue].AcceptorInfo.AcceptorFilePaths.Count() > 0) ? checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo[mostRecentAttemptIndexValue].AcceptorInfo.AcceptorFilePaths.Count() - 1 : 0;
                    mostRecentAttachedApproverFilePathIndexValue = (checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo[mostRecentAttemptIndexValue].ApproverInfo.ApproverFilePaths.Count() > 0) ? checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo[mostRecentAttemptIndexValue].ApproverInfo.ApproverFilePaths.Count() - 1 : 0;
                    mostRecentAttemptResponseInfo = checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].QuestionAttemptResponseInfo[mostRecentAttemptIndexValue];
                }
                ChecklistIndexValues checklistIndexValues = new ChecklistIndexValues()
                {
                    MostRecentAttemptIndexValue = mostRecentAttemptIndexValue,
                    MostRecentAttachedAcceptorFilePathIndexValue = mostRecentAttachedAcceptorFilePathIndexValue,
                    MostRecentAttachedApproverFilePathIndexValue = mostRecentAttachedApproverFilePathIndexValue,
                    MostRecentAttemptResponseInfo = mostRecentAttemptResponseInfo,
                };
                checklistModel.SubSectionModel.ListQuestionAndResponseInfo[i].ChecklistIndexValues = checklistIndexValues;
            }

            return checklistModel;
        }
        public PaymentAuthorisationModel getPaymentAuthorisationModel(BQCSubSections SubSection, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                bool isComplete = db.BQCSubSectionSignatures.Any(r => r.BQCSubSectionID == SubSection.BQCSubSectionID && r.PlotID == PlotID);
                SignaturePadModel signatureInfo = getSignedDetails(SubSection.BQCSubSectionID, PlotID);
                bool completionIsDisabled = checkIfPaymentAuthCompletionShouldBeDisabled(PlotID, SubSection.BQCSubSectionID);
                bool isEditable = (!isComplete && !completionIsDisabled);
                PaymentAuthorisationModel paymentAuthModel = new PaymentAuthorisationModel()
                {
                    SignatureName = signatureInfo.SignedBy,
                    SignedDateTime = signatureInfo.SignedDateTime,
                    SignatureImage = signatureInfo.SignatureImage,
                    FullName = identityDictionary.getFullNameOfCurrentUser(),
                    SubSectionID = SubSection.BQCSubSectionID,
                    completionIsDisabled = completionIsDisabled,
                    isEditable = isEditable,
                    isComplete = isComplete,
                };

                //check if complete
                if (isComplete)
                {
                    paymentAuthModel.SignatureImage = signatureInfo.SignatureImage;
                    paymentAuthModel.SignatureName = signatureInfo.SignedBy;
                    paymentAuthModel.SignedDateTime = signatureInfo.SignedDateTime;
                    paymentAuthModel.isComplete = true;                 
                }
                return paymentAuthModel;
            }
        }
        public SubSectionModel getTradeContractorChecklistStaffApprovalModel(BQCSubSections SubSection, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int statusWorkerSubSectionCategoryEnum = getStatusOfTradeContractorSubSection(SubSection.BQCSubSectionID, PlotID);
                string statusWorkerSubSectionString = ((StatusOfTradeContractorSubSection)statusWorkerSubSectionCategoryEnum).AsString(EnumFormat.Description);
                SignaturePadModel signatureInfo = getSignedDetails(SubSection.BQCSubSectionID, PlotID);
                bool isComplete = (statusWorkerSubSectionCategoryEnum == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised || statusWorkerSubSectionCategoryEnum == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised);
                bool isEditable = (statusWorkerSubSectionCategoryEnum == (int)StatusOfTradeContractorSubSection.AwaitingResponseFromStaff);

                SubSectionModel subSectionModel = new SubSectionModel() 
                {
                    SignatureImage = signatureInfo.SignatureImage,
                    SignatureDateTime = signatureInfo.SignedDateTime,
                    SignatureName = signatureInfo.SignedBy,
                    SubSectionID = SubSection.BQCSubSectionID,
                    ListQuestionAndResponseInfo = getListOfQuestionsAndResponseInfoForSubsection(SubSection.BQCSubSectionID, PlotID),
                    SubSectionStatusEnumValue = statusWorkerSubSectionCategoryEnum,
                    SubSectionStatusString = statusWorkerSubSectionString,
                    isComplete = isComplete,
                    isEditable = isEditable,
                    CurrentUserFullName = identityDictionary.getFullNameOfCurrentUser(),
                };

                //BQCSubSectionSignatures signature = db.BQCSubSectionSignatures.Where(r => r.BQCSubSectionID == SubSection.BQCSubSectionID && r.PlotID == PlotID).FirstOrDefault();
                //if (signature != null)
                //{
                //    subSectionModel.SignatureName = signature.Name;
                //    subSectionModel.SignatureDateTime = signature.SignedDateTime;
                //    //subSectionModel.isComplete = isSiteSubSectionCompleted(PlotID, SubSectionID);
                //}
                return subSectionModel;
            }
        }
        public SubSectionModel getStaffChecklistModel(BQCSubSections SubSection, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                BQCSubSections subSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSection.BQCSubSectionID).FirstOrDefault();
                SignaturePadModel signatureInfo = getSignedDetails(SubSection.BQCSubSectionID, PlotID);
                int subSectionStatusEnumValue = getStatusOfStaffSubSection(SubSection.BQCSubSectionID, PlotID);
                string subSectionStatusString = ((StatusOfStaffSubSection)subSectionStatusEnumValue).AsString(EnumFormat.Description);
                bool isEditable = (subSectionStatusEnumValue != (int)StatusOfStaffSubSection.Complete);
                bool isComplete = (subSectionStatusEnumValue == (int)StatusOfStaffSubSection.Complete);

                SubSectionModel subSectionModel = new SubSectionModel()
                {
                    SignatureImage = signatureInfo.SignatureImage,
                    SignatureName = signatureInfo.SignedBy,
                    SignatureDateTime = signatureInfo.SignedDateTime,
                    SubSectionID = subSection.BQCSubSectionID,
                    SectionID = subSection.BQCSectionID,
                    SubSectionStatusEnumValue = subSectionStatusEnumValue,
                    SubSectionStatusString = subSectionStatusString,
                    isEditable = isEditable,
                    isComplete = isComplete,
                    CurrentUserFullName = identityDictionary.getFullNameOfCurrentUser(),
                };

                subSectionModel.ListQuestionAndResponseInfo = getListOfQuestionsAndResponseInfoForSubsection(subSection.BQCSubSectionID, PlotID);

                return subSectionModel;
            }
        }
        public SignaturePadModel getSignedDetails(int subsectionId, int plotId)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int? roleEnum = db.BQCSubSections.Where(r => r.BQCSubSectionID == subsectionId).Select(r => r.RoleNameEnum).FirstOrDefault();
                BQCSubSectionSignatures signatureInfo = db.BQCSubSectionSignatures.Where(r => r.BQCSubSectionID == subsectionId && r.RoleNameEnum == roleEnum && r.PlotID == plotId).OrderByDescending(r=>r.SignedDateTime).FirstOrDefault();
                SignaturePadModel signaturePadInfo = new SignaturePadModel();
                if (signatureInfo != null)
                {
                    signaturePadInfo.SignedDateTime = signatureInfo.SignedDateTime;
                    signaturePadInfo.SignatureImage = signatureInfo.Signature;
                    signaturePadInfo.SignedBy = signatureInfo.Name ?? "user";
                }
                return signaturePadInfo;
            }
        }
        public bool checkIfPaymentAuthCompletionShouldBeDisabled(int PlotID, int SubSectionID)
        {
            //used to check if payment auth completion should be disabled because of previous subsection completions
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int SectionID = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.BQCSectionID).FirstOrDefault();
                int? tradeContractorSubSectionID = getTradeContractorSubSectionIDIfPresentInSection(SectionID);
                int? tradeContractorSubSectionStatus = getStatusOfTradeContractorSubSection((int)tradeContractorSubSectionID, PlotID);
                bool workerSubSectionCompletedAndApproved = (tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                bool paymentAuthCompletionIsDisabled = (!workerSubSectionCompletedAndApproved);

                return paymentAuthCompletionIsDisabled;
            }
        }
        public ChecklistModel getTradeContractorChecklistModel(int PlotID, int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int siteID = getSiteIDFromPlotID(PlotID);
                PlotInformationModel plotInfo = getPlotInformationModel(PlotID);
                SiteInformationModel siteInfo = getSiteInformationModel(siteID);
                ChecklistModel checklistModel = new ChecklistModel()
                {
                    SectionID = SectionID,
                    SectionNaming = getListSectionDetails(PlotID),
                    SiteAndPlotInfo = new SiteAndPlotInformationModel() { SiteInfo = siteInfo, PlotInfo = plotInfo},
                };

                int? tradeContractorSubSection = getTradeContractorSubSectionIDIfPresentInSection(SectionID);
                BQCSubSections subSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == tradeContractorSubSection).FirstOrDefault();

                //set read only as variable so can change later looping through questions
                int statusOfSubSection = getStatusOfTradeContractorSubSection(subSection.BQCSubSectionID, PlotID);
                
                string statusOfSubSectionString = ((StatusOfTradeContractorSubSection)statusOfSubSection).AsString(EnumFormat.Description);
                bool isEditable = (statusOfSubSection == (int)StatusOfTradeContractorSubSection.AmendmentByTradeContractorNeeded || statusOfSubSection == (int)StatusOfTradeContractorSubSection.IncompleteByTradeContractor);
                bool isComplete = (statusOfSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || statusOfSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                SignaturePadModel signatureInfo = getSignedDetails(subSection.BQCSubSectionID, PlotID);

                //prepare subsection model
                SubSectionModel subSectionModel = new SubSectionModel()
                {
                    SignatureImage = signatureInfo.SignatureImage,
                    SignatureDateTime = signatureInfo.SignedDateTime,
                    SignatureName = signatureInfo.SignedBy,
                    SectionFullName = getSectionFullNameFromSectionID(SectionID),
                    SubSectionID = subSection.BQCSubSectionID,
                    SectionID = subSection.BQCSectionID,
                    SubSectionStatusEnumValue = statusOfSubSection,
                    SubSectionStatusString = statusOfSubSectionString,
                    isEditable = isEditable,
                    isComplete = isComplete,
                    CurrentUserFullName = identityDictionary.getFullNameOfCurrentUser(),
                };

                subSectionModel.ListQuestionAndResponseInfo = getListOfQuestionsAndResponseInfoForSubsection(subSection.BQCSubSectionID, PlotID);

                checklistModel.SubSectionModel = subSectionModel;
                checklistModel = getChecklistIndexValues(checklistModel);

                return checklistModel;
            }
        }
        public SectionModel getSubSectionsForStaffSection(int SiteID, int PlotID, int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                PlotInformationModel plotInfo = getPlotInformationModel(PlotID);
                SiteInformationModel siteInfo = getSiteInformationModel(SiteID);
                SectionModel sectionModel = new SectionModel()
                {
                    SitePlotInfo = new SiteAndPlotInformationModel() { SiteInfo = siteInfo, PlotInfo = plotInfo },
                    SectionCategoryName = getSectionFullNameFromSectionID(SectionID),
                    SubSectionIDToLoad = getSubSectionToLoadNextInSection(PlotID, SectionID),
                    SectionIDToLoad = SectionID,
                };

                List<SubSectionInfoForButtonsModel> listSubSectionInfoForButtons = new List<SubSectionInfoForButtonsModel>();
                //load sub section ids and names
                List<BQCSubSections> SubSectionsInSection = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID).ToList();
                foreach (BQCSubSections subsection in SubSectionsInSection)
                {
                    SubSectionInfoForButtonsModel subSectionInfo = getSubSectionInfoForButtons(subsection, PlotID);

                    listSubSectionInfoForButtons.Add(subSectionInfo);
                }

                sectionModel.SubSectionInfoForButtons = listSubSectionInfoForButtons;
                sectionModel.SectionNaming = getListSectionDetails(PlotID);

                return sectionModel;
            }
        }
        public List<QuestionAndResponseInfoModel> getListOfQuestionsAndResponseInfoForSubsection(int SubsectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<QuestionAndResponseInfoModel> listQuestionAndResponseInfo = new List<QuestionAndResponseInfoModel>();

                List<BQCQuestions> subSectionQuestions = db.BQCQuestions.Where(r => r.BQCSubSectionID == SubsectionID).ToList();

                foreach (BQCQuestions question in subSectionQuestions)
                {
                    QuestionAndResponseInfoModel questionAndResponseInfo = getQuestionAndResponseInfoForQuestion(question, PlotID, SubsectionID);

                    listQuestionAndResponseInfo.Add(questionAndResponseInfo);
                }

                return listQuestionAndResponseInfo;
            }
        }
        public QuestionAndResponseInfoModel getQuestionAndResponseInfoForQuestion(BQCQuestions Question, int PlotID, int SubSectionId)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //setup checklist models
                List<QuestionAttemptResponseInfo> allQuestionAttemptHistory = new List<QuestionAttemptResponseInfo>();

                ////get attempt records
                allQuestionAttemptHistory = getAttemptHistoryOfQuestion(PlotID, Question);
                int questionAcceptorResponseEnum = getAcceptorResponseEnum(PlotID, Question.BQCQuestionID);
                string questionAcceptorResponseString = ((AcceptorResponseTypes)questionAcceptorResponseEnum).AsString(EnumFormat.Description);
                int questionApproverResponseEnum = getApproverResponseEnum(PlotID, Question.BQCQuestionID);
                string questionApproverResponseString = ((ApprovalResponseTypes)questionApproverResponseEnum).AsString(EnumFormat.Description);
                int questionStatusForTradeEnum = getQuestionStatusForTradeEnum(PlotID, Question.BQCQuestionID);
                string questionStatusForTradeString = "";

                //fill model
                QuestionAndResponseInfoModel questionAndResponseInfo = new QuestionAndResponseInfoModel()
                {
                    QuestionAttemptResponseInfo = allQuestionAttemptHistory,
                    QuestionAttemptCount = getQuestionAttemptCount(false, PlotID, Question.BQCQuestionID),
                    QuestionAttemptCountForFileSave = getQuestionAttemptCount(true, PlotID, Question.BQCQuestionID),
                    QuestionContent = Question.QuestionText,
                    ChecklistQuestionID = Question.BQCQuestionID,
                    QuestionAcceptorResponseEnum = questionAcceptorResponseEnum,
                    QuestionAcceptorResponseString = questionAcceptorResponseEnum,
                    QuestionApproverResponseEnum = questionApproverResponseEnum,
                    QuestionApproverResponseString = questionApproverResponseString,
                    QuestionStatusEnumForTradeContractor = questionStatusForTradeEnum,
                    QuestionStatusStringForTradeContractor = questionStatusForTradeString,
                    QuestionApprovalAlertColor = getQuestionApprovalAlertColor(questionApproverResponseEnum, questionAcceptorResponseEnum, SubSectionId),
                    QuestionAcceptorAlertColor = getQuestionAcceptorAlertColor(questionAcceptorResponseEnum, SubSectionId),
                };

                return questionAndResponseInfo;
            }
        }
        public string getQuestionApprovalAlertColor(int questionApproverResponseEnum, int questionAcceptorResponseEnum, int subsectionId)
        {
            int subsectionType = getSubSectionTypeEnum(null, subsectionId);
            var approvalAlertColor = "";
            if (questionAcceptorResponseEnum == (int)AcceptorResponseTypes.Incomplete)
            {
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    approvalAlertColor = "grey";
                }
                else //staff
                {
                    approvalAlertColor = "orange";
                }
            }
            else if (questionApproverResponseEnum == (int)ApprovalResponseTypes.Approve)
            {
                approvalAlertColor = "green";
            }
            else if (questionApproverResponseEnum == (int)ApprovalResponseTypes.SubjectToConditions)
            {
                approvalAlertColor = "orange";
            }
            else if (questionApproverResponseEnum == (int)ApprovalResponseTypes.Incomplete)
            {
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    if (questionAcceptorResponseEnum == (int)AcceptorResponseTypes.Incomplete)
                    {
                        approvalAlertColor = "grey";
                    } 
                    else
                    {
                        approvalAlertColor = "orange";
                    }
                }
                else //staff
                {
                    if (questionAcceptorResponseEnum == (int)AcceptorResponseTypes.Incomplete)
                    {
                        approvalAlertColor = "orange";
                    }
                    else
                    {
                        approvalAlertColor = "grey";
                    }
                }
            }
            else if (questionApproverResponseEnum == (int)ApprovalResponseTypes.Reject)
            {
                approvalAlertColor = "red";
            }
            return approvalAlertColor;
        }
        public string getQuestionAcceptorAlertColor(int questionAcceptResponseEnum, int subsectionId)
        {
            var acceptorAlertColor = "grey";
            if (questionAcceptResponseEnum == (int)AcceptorResponseTypes.Accepted)
            {
                acceptorAlertColor = "green";
            }
            return acceptorAlertColor;
        }
        public List<QuestionAttemptResponseInfo> getAttemptHistoryOfQuestion(int PlotID, BQCQuestions Question)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //prepare attempt history
                var allQuestionAttemptHistory = new List<QuestionAttemptResponseInfo>();
                //get all responses for question
                List<BQCResponses> allResponses = db.BQCResponses.Where(r => r.PlotID == PlotID && r.BQCQuestionID == Question.BQCQuestionID).OrderBy(r => r.BQCResponseID).ToList();
                //loop through responses
                for (int i = 0; i < allResponses.Count(); i++)
                {
                    //prepare model and add to list
                    AcceptorInfo acceptorInfo = getAcceptorInfo(allResponses[i], i + 1);
                    ApproverInfo approverInfo = getApproverInfo(allResponses[i], i + 1);

                    QuestionAttemptResponseInfo questionAttempt = new QuestionAttemptResponseInfo()
                    {
                        AttemptNumber = i + 1,
                        AcceptorInfo = acceptorInfo,
                        ApproverInfo = approverInfo,
                    };

                    allQuestionAttemptHistory.Add(questionAttempt);
                }

                return allQuestionAttemptHistory;
            }
        }
        public AcceptorInfo getAcceptorInfo(BQCResponses Response, int AttemptNumber)
        {
            AcceptorInfo acceptorInfo = new AcceptorInfo();

            acceptorInfo.Name = Response.AcceptorName??"user";
            acceptorInfo.AcceptorAcknowledged = Response.AcceptorWorkCompleted;
            if (Response.AcceptorWorkCompletedComment != null)
            {
                acceptorInfo.Comment = Response.AcceptorWorkCompletedComment;
            }
            acceptorInfo.SignedDateTime = Response.AcceptorWorkCompletedDateTime;
            acceptorInfo.AcceptorFilePaths = getAcceptorFilePaths(AttemptNumber, Response.BQCQuestionID, Response.PlotID);

            return acceptorInfo;
        }
        public ApproverInfo getApproverInfo(BQCResponses Response, int AttemptNumber)
        {
            ApproverInfo approverInfo = new ApproverInfo()
            {
                ApproverResponseEnum = Response.ApproverResponseEnumValue,
                ApproverResponseString = ((ApprovalResponseTypes)Response.ApproverResponseEnumValue).AsString(EnumFormat.Description),
                ApproverName = identityDictionary.getFullNameOfUserFromStaffUserID(Response.ApproverUserID),
            };

            // if approved datetime is not empty set model value
            if (Response.ApproverResponseDateTime != null)
            {
                approverInfo.SignedDateTime = (DateTime)Response.ApproverResponseDateTime;
            }
            //if there is a comment, give to model
            if (Response.ApproverResponseComment != null)
            {
                approverInfo.Comment = Response.ApproverResponseComment;
            }
            approverInfo.ApproverFilePaths = getApproverFilePaths(AttemptNumber, Response.BQCQuestionID, Response.PlotID);

            return approverInfo;
        }
        public List<string> getAcceptorFilePaths(int AttemptNumber, int QuestionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //files where role is not equal to site
                List<string> acceptorFilePaths = db.BQCFiles.Where(r => r.AttemptCounter == AttemptNumber && r.BQCQuestionID == QuestionID && r.AcceptorOrApproverEnum == (int)AcceptorOrApprover.acceptor && r.PlotID == PlotID).Select(r => filePathPrefix + r.FilePath).ToList();
                return acceptorFilePaths;
            }
        }
        public List<string> getApproverFilePaths(int AttemptNumber, int QuestionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //files where role is not equal to site
                List<string> approverFilePaths = db.BQCFiles.Where(r => r.AttemptCounter == AttemptNumber && r.BQCQuestionID == QuestionID && r.AcceptorOrApproverEnum == (int)AcceptorOrApprover.approver && r.PlotID == PlotID).Select(r => filePathPrefix+r.FilePath).ToList();
                return approverFilePaths;
            }
        }
        public int getAcceptorResponseEnum(int PlotID, int QuestionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int statusOfAcceptorResponseEnumValue = (int)AcceptorResponseTypes.Incomplete;
                //kept as incomplete if not been accepted at all by worker, or approval response received from staff and not yet re-accepted

                //check if worker amendment is needed
                BQCResponses mostRecentResponse = db.BQCResponses.Where(r => r.BQCQuestionID == QuestionID && r.PlotID == PlotID).OrderByDescending(r => r.BQCResponseID).FirstOrDefault();

                if (mostRecentResponse != null && mostRecentResponse.AcceptorWorkCompleted == true)
                {
                    //if there is an acceptor response without an approver response
                    statusOfAcceptorResponseEnumValue = (int)AcceptorResponseTypes.Accepted;
                }

                return statusOfAcceptorResponseEnumValue;
            }
        }
        public int getApproverResponseEnum(int PlotID, int QuestionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int approverResponseEnum = (int)ApprovalResponseTypes.Incomplete;
                BQCResponses mostRecentResponse = db.BQCResponses.Where(r => r.BQCQuestionID == QuestionID && r.PlotID == PlotID).OrderByDescending(r => r.BQCResponseID).FirstOrDefault();
                if (mostRecentResponse != null)
                {
                    if (mostRecentResponse.ApproverResponseDateTime != null)
                    {
                        approverResponseEnum = mostRecentResponse.ApproverResponseEnumValue;
                    }
                }

                return approverResponseEnum;
            }
        }
        public int getQuestionStatusForTradeEnum(int PlotID, int QuestionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                BQCResponses mostRecentResponse = db.BQCResponses.Where(r => r.BQCQuestionID == QuestionID && r.PlotID == PlotID).OrderByDescending(r => r.BQCResponseID).FirstOrDefault();
                if(mostRecentResponse != null)
                {
                    bool acceptedButNotRespondedTo = (mostRecentResponse.AcceptorWorkCompleted && mostRecentResponse.ApproverResponseEnumValue == (int)ApprovalResponseTypes.Incomplete); ;
                    bool approvedByStaff = (mostRecentResponse.ApproverResponseEnumValue == (int)ApprovalResponseTypes.Approve || mostRecentResponse.ApproverResponseEnumValue == (int)ApprovalResponseTypes.SubjectToConditions);
                    bool markedAsConditionalByStaff = (mostRecentResponse.ApproverResponseEnumValue == (int)ApprovalResponseTypes.Approve);
                    bool awaitingCompletionFromTradeContractor = (mostRecentResponse.ApproverResponseEnumValue == (int)ApprovalResponseTypes.Reject || mostRecentResponse.AcceptorWorkCompleted == false);
                    if (acceptedButNotRespondedTo)
                    {
                        return (int)StatusOfQuestionForTradeContractor.AwaitingStaffResponse;
                    }
                    else if (approvedByStaff)
                    {
                        return (int)StatusOfQuestionForTradeContractor.Approved;
                    }
                    else if (markedAsConditionalByStaff)
                    {
                        return (int)StatusOfQuestionForTradeContractor.Conditional;
                    }
                    else if (awaitingCompletionFromTradeContractor)
                    {
                        return (int)StatusOfQuestionForTradeContractor.AwaitingTradeContractorCompletion;
                    }
                    else
                    {
                        return (int)StatusOfQuestionForTradeContractor.AwaitingTradeContractorCompletion;
                    }
                }
                else
                {
                    return (int)StatusOfQuestionForTradeContractor.AwaitingTradeContractorCompletion;
                }
            }
        }
        public int getQuestionApprovalStatusForStaffEnum(int questionAcceptorResponseEnum, int questionApproverResponseEnum)
        {
            return 0;
        }
        public int getQuestionAttemptCount(bool ForFileSave, int PlotID, int QuestionID)
        {
            //file save is fifferent because if most recent response has an approver response then attempt must be set to +1 for next acceptor attempt
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                var attemptCount = 1;
                List<BQCResponses> allResponses = db.BQCResponses.Where(r => r.PlotID == PlotID && r.BQCQuestionID == QuestionID).ToList();
                if (ForFileSave)
                {
                    if (allResponses.Count != 0)
                    {
                        if (allResponses[allResponses.Count() - 1].ApproverResponseEnumValue != (int)ApprovalResponseTypes.Incomplete)
                        {
                            attemptCount = allResponses.Count() + 1;
                        }
                        else
                        {
                            attemptCount = allResponses.Count();
                        }
                    }
                }
                else
                {
                    attemptCount = allResponses.Count();
                }

                return attemptCount;
            }
        }
        public int geSectionIDToLoadNextFromPlot(int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                //current build section
                int mostRecentSubSectionSignatureSubSectionID = db.BQCSubSectionSignatures.Where(r => r.PlotID == PlotID).OrderByDescending(r => r.SignedDateTime).Select(r => r.BQCSubSectionID).FirstOrDefault();

                //default to first incase no records found
                int currentSectionID = 1;

                if (mostRecentSubSectionSignatureSubSectionID != null && mostRecentSubSectionSignatureSubSectionID != 0)
                {
                    currentSectionID = db.BQCSubSections.Where(r => r.BQCSubSectionID == mostRecentSubSectionSignatureSubSectionID).Select(r => r.BQCSectionID).FirstOrDefault();
                    BQCSections currentSection = db.BQCSections.Where(r => r.BQCSectionID == currentSectionID).FirstOrDefault();

                    int finalSubSectionIDInSection = db.BQCSubSections.Where(r => r.BQCSectionID == currentSectionID).OrderByDescending(r => r.SortOrderValue).Select(r => r.BQCSubSectionID).FirstOrDefault();

                    bool itIsFinalSubSectionInSection = (mostRecentSubSectionSignatureSubSectionID == finalSubSectionIDInSection);
                    bool thereIsAProceedingSection = (db.BQCSections.Any(r=>r.SortOrderValue == currentSection.SortOrderValue + 1));
                    if (itIsFinalSubSectionInSection && thereIsAProceedingSection)
                    {
                        currentSectionID++;
                    }
                }
                int? plotStaringSectionId = getPlotSectionStartingPoint(PlotID);
                if(plotStaringSectionId != null)
                {
                    if (isFirstSectionSortOrderValueBeforeSecond(currentSectionID, (int)plotStaringSectionId))
                    {
                        currentSectionID = (int)plotStaringSectionId;
                    }
                }
                return currentSectionID;
            }
        }
        public bool isFirstSectionSortOrderValueBeforeSecond(int firstSectionid, int secondSectionId)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int firstSectionSortOrderValue = db.BQCSections.Where(r => r.BQCSectionID == firstSectionid).Select(r => r.SortOrderValue).FirstOrDefault();
                int secondSectionSortOrderValue = db.BQCSections.Where(r => r.BQCSectionID == secondSectionId).Select(r => r.SortOrderValue).FirstOrDefault();
                return (firstSectionSortOrderValue < secondSectionSortOrderValue);
            }
        }
        public int? getPlotSectionStartingPoint(int PlotId)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext()) {
                return db.BQCPlotSectionStartingPoints.Where(r => r.PlotId == PlotId).Select(r => r.StartingAtSectionId).FirstOrDefault();
            }
        }
        public SubSectionInfoForButtonsModel getSubSectionInfoForButtons(BQCSubSections SubSection, int PlotID)
        {
            string subSectionButtonName = (SubSection.RoleNameEnum == null) ? "PaymentAuthorisation": ((AllRoles)SubSection.RoleNameEnum).AsString(EnumFormat.Description);
            SubSectionInfoForButtonsModel subSectionInfo = new SubSectionInfoForButtonsModel()
            {
                SubSectionID = SubSection.BQCSubSectionID,
                SubSectionButtonName = subSectionButtonName,
                SectionID = SubSection.BQCSubSectionID,
            };

            int subSectionType = getSubSectionTypeEnum(SubSection, null);

            //check if subsections are complete
            StatusOfSubSectionButtonState subSectionStatusForButton = 0;

            if (subSectionType == (int)SubSectionType.StaffChecklist)
            {
                int subSectionStatus = getStatusOfStaffSubSection(SubSection.BQCSubSectionID, PlotID);
                subSectionStatusForButton = (subSectionStatus == (int)StatusOfStaffSubSection.Complete) ? StatusOfSubSectionButtonState.Complete : StatusOfSubSectionButtonState.Incomplete;
            }
            if (subSectionType == (int)SubSectionType.PaymentAuthorisation)
            {
                int subSectionStatus = getStatusOfPaymentAuthSubSection(SubSection.BQCSubSectionID, PlotID);
                subSectionStatusForButton = (subSectionStatus == (int)StatusOfPaymentAuthorisationSubSection.PaymentAuthorised) ? StatusOfSubSectionButtonState.Complete : StatusOfSubSectionButtonState.Incomplete;
                //change button name from role to payment auth
                subSectionInfo.SubSectionButtonName = "Payment Authorisation";
            }
            if (subSectionType == (int)SubSectionType.TradeContractorChecklist)
            {
                int subSectionStatus = getStatusOfTradeContractorSubSection(SubSection.BQCSubSectionID, PlotID);

                switch ((StatusOfTradeContractorSubSection)subSectionStatus)
                {
                    case StatusOfTradeContractorSubSection.SubSectionDependenciesIncompleted:
                        subSectionStatusForButton = StatusOfSubSectionButtonState.Incomplete;
                        break;
                    case StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised:
                    case StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised:
                        subSectionStatusForButton = StatusOfSubSectionButtonState.Complete;
                        break;
                    case StatusOfTradeContractorSubSection.IncompleteByTradeContractor:
                        subSectionStatusForButton = StatusOfSubSectionButtonState.Incomplete;
                        break;
                    case StatusOfTradeContractorSubSection.AwaitingResponseFromStaff:
                        subSectionStatusForButton = StatusOfSubSectionButtonState.AwaitingAmendmentFromStaff;
                        break;
                    case StatusOfTradeContractorSubSection.AmendmentByTradeContractorNeeded:
                        subSectionStatusForButton = StatusOfSubSectionButtonState.AwaitingAmendmentFromWorker;
                        break;
                }
            }

            subSectionInfo.SubSectionStatusForButtonState = (int)subSectionStatusForButton;

            return subSectionInfo;
        }
        public int getSubSectionToLoadNextInSection(int PlotID, int SectionIDToLoad)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<BQCSubSections> SubSectionsInSection = db.BQCSubSections.Where(r => r.BQCSectionID == SectionIDToLoad).OrderByDescending(r => r.BQCSubSectionID).ToList();

                //find first subsection to load first 
                int SubSectionToLoad = SubSectionsInSection.OrderBy(r => r.SortOrderValue).Select(r => r.BQCSubSectionID).FirstOrDefault();

                foreach (BQCSubSections subsection in SubSectionsInSection)
                {
                    bool thereIsAProceedingSubSectionWithinSameSection = (SubSectionsInSection.Any(r => r.SortOrderValue == subsection.SortOrderValue + 1));

                    int subSectionType = getSubSectionTypeEnum(subsection, null);
                    if (subSectionType == (int)SubSectionType.StaffChecklist)
                    {
                        if (getStatusOfStaffSubSection(subsection.BQCSubSectionID, PlotID) == (int)StatusOfStaffSubSection.Complete)
                        {
                            if (thereIsAProceedingSubSectionWithinSameSection)
                            {
                                SubSectionToLoad = subsection.BQCSubSectionID + 1;
                            }
                        }
                    } 
                    else if (subSectionType == (int)SubSectionType.TradeContractorChecklist)
                    {
                        var status = getStatusOfTradeContractorSubSection(subsection.BQCSubSectionID, PlotID);
                        bool workerSubSectionCompleted = (status == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised || status == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised);

                        if (workerSubSectionCompleted)
                        {
                            SubSectionToLoad = getSubSectionIDAfterTradeContractorSubSection(PlotID, subsection.BQCSubSectionID);
                        }
                    }
                    else if (subSectionType == (int)SubSectionType.PaymentAuthorisation)
                    {
                        if (getStatusOfPaymentAuthSubSection(subsection.BQCSubSectionID, PlotID) == (int)StatusOfPaymentAuthorisationSubSection.PaymentAuthorised)
                        {
                            SubSectionToLoad = subsection.BQCSubSectionID;
                        }
                    }
                }

                return SubSectionToLoad;
            }
        }
        public int getSubSectionIDAfterTradeContractorSubSection(int PlotID, int SubSectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int SubSectionIDToLoad = 0;
                int SectionID = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.BQCSectionID).FirstOrDefault();
                BQCSubSections PaymentAuthSubSection = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.PaymentAuthorisation).FirstOrDefault();
                if (PaymentAuthSubSection != null)
                {
                    SubSectionIDToLoad = PaymentAuthSubSection.BQCSubSectionID;
                }
                else
                {
                    List<BQCSections> allSections = db.BQCSections.ToList();
                    //get index of section to check that there is another section after
                    int IndexOfSectionID = allSections.FindIndex(r => r.BQCSectionID == SectionID);
                    //get section after using index
                    int SectionIDOfSectionAfter = allSections[IndexOfSectionID + 1].BQCSectionID;
                    //get subsection to load in new section
                    SubSectionIDToLoad = getSubSectionToLoadNextInSection(PlotID, SectionID);
                }

                return SubSectionIDToLoad;
            }
        }
        public int? getTotalBuildStagesCompletedCountInPlot(int PlotID)
        {
            using (CroudaceContext db = new CroudaceContext())
            {
                return db.SitePlotBuildStages.Where(r => r.SitePlotID == PlotID).OrderByDescending(r => r.BuildStageID).Select(r => r.StageNo).FirstOrDefault() ?? 0;
            }
        }
        public string getTotalBuildStagesCompletedAndTotalStagesCountInPlot(int PlotID)
        {
            int? totalBuildStagesCompleted = getTotalBuildStagesCompletedCountInPlot(PlotID);
            return totalBuildStagesCompleted + "/" + totalNumberOfBuildStages;
        }
        public int getTotalBuildSectionsCompletedCountInPlot(int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int totalBuildSectionsCompleted = 0;
                List<BQCSections> allAppropriateSections = getSectionsListForPlot(PlotID);
                foreach(BQCSections section in allAppropriateSections)
                {
                    if(isSectionCompleted(section.BQCSectionID, PlotID))
                    {
                        totalBuildSectionsCompleted++;
                    }
                }
                return totalBuildSectionsCompleted;
            }
        }
        //public int getCurrentBuildSectionCountInPlot(int PlotID)
        //{
        //    using (SiteManagementSystemContext db = new SiteManagementSystemContext())
        //    {
        //        int totalSectionsCompletedInPlot = getTotalBuildSectionsCompletedCountInPlot(PlotID);

        //        //check if final sub section signature in section
        //        int finalSubSectionIDInSection = db.BQCSubSections.Where(r => r.BQCSectionID == totalSectionsCompletedInPlot).OrderByDescending(r => r.SortOrderValue).Select(r => r.BQCSubSectionID).FirstOrDefault();

        //        //set to next section if is final sub section signature
        //        if (totalSectionsCompletedInPlot == finalSubSectionIDInSection)
        //        {
        //            totalSectionsCompletedInPlot++;
        //        }

        //        return totalSectionsCompletedInPlot;
        //    }
        //}
        //public string getCurrentBuildSectionCategoryNameInPlot(int PlotID)
        //{
        //    using (SiteManagementSystemContext db = new SiteManagementSystemContext())
        //    {
        //        int currentBuildSectionCount = getCurrentBuildSectionCountInPlot(PlotID);
        //        int sectionCategoryNameEnumValue = db.BQCSections.Where(r => r.BQCSectionID == currentBuildSectionCount).Select(r => r.SectionCategoryEnum).FirstOrDefault();

        //        return ((SectionCategoryNames)sectionCategoryNameEnumValue).AsString(EnumFormat.Description);
        //    }
        //}
        public int getTotalBuildSectionsCount(int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<BQCSections> allAppropriateSections = getSectionsListForPlot(PlotID);
                return allAppropriateSections.Count();
            }
        }
        public string getTotalBuildSectionsCompletedAndTotalInPlot(int PlotID)
        {
            int totalSectionsCompleted = getTotalBuildSectionsCompletedCountInPlot(PlotID);
            int totalSectionsCount = getTotalBuildSectionsCount(PlotID);
            return getTotalBuildSectionsCompletedAndTotalInPlotString(totalSectionsCompleted, totalSectionsCount);
        }
        public string getTotalBuildSectionsCompletedAndTotalInPlotString(int totalSectionsCompleted, int totalSectionsInPlot)
        {
            //extra sections not applicable to plot completed before update?
            if (totalSectionsCompleted > totalSectionsInPlot)
            {
                totalSectionsCompleted = totalSectionsInPlot;
            }
            return totalSectionsCompleted + "/" + totalSectionsInPlot;
        }
        public int getStatusOfPaymentAuthorisationSection(int PlotID, int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                bool sectionHasPaymentAuthorisationSubSection = (getPaymentAuthorisationSubSectionIDIfPresentInSection(SectionID)!=null);

                if (!sectionHasPaymentAuthorisationSubSection)
                {
                    return (int)StatusOfPaymentAuthorisationSubSection.PaymentAuthorisationNotApplicableToSection;
                }
                else
                {
                    int subSectionIDOfPaymentAuthorisation = (int)getPaymentAuthorisationSubSectionIDIfPresentInSection(SectionID);
                    bool paymentHasBeenAuthorisedForSection = db.BQCSubSectionSignatures.Any(r => r.BQCSubSectionID == subSectionIDOfPaymentAuthorisation && r.PlotID == PlotID);
                    return (paymentHasBeenAuthorisedForSection) ? (int)StatusOfPaymentAuthorisationSubSection.PaymentAuthorised : (int)StatusOfPaymentAuthorisationSubSection.PaymentNotYetAuthorised;
                }
            }
        }
        public int getStatusOfPlot(int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int sectionsCompletedAndApprovedCounter = 0;
                List<int> allSectionIDs = db.BQCSections.Select(r => r.BQCSectionID).ToList();
                foreach (int sectionID in allSectionIDs)
                {
                    int statusOfSection = getStatusOfSection(sectionID, PlotID);
                    bool completedByTradeContractorAndApprovedByStaff = (statusOfSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || statusOfSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                    if (completedByTradeContractorAndApprovedByStaff) { sectionsCompletedAndApprovedCounter++; }
                }
                return (sectionsCompletedAndApprovedCounter == allSectionIDs.Count()) ? (int)StatusOfPlot.AllSectionsCompletedByTradeContractorAndApprovedBySiteManagement : (int)StatusOfPlot.NotAllSectionsCompletedByTradeContractorAndApprovedBySiteManagement;
            }
        }
        public int? getSiteSubSectionIDFromSectionID(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                var subsectionID = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.StaffChecklist).Select(r => r.BQCSubSectionID).FirstOrDefault();
                return subsectionID;
            }
        }
        public int? getTradeContractorSubSectionIDIfPresentInSection(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int? subsectionId = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.TradeContractorChecklist).Select(r => r.BQCSubSectionID).FirstOrDefault();
                return (subsectionId == 0) ? null : subsectionId;
            }
        }
        public int? getStaffSubSectionIDIfPresentInSection(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int? subsectionId = db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.StaffChecklist).Select(r => r.BQCSubSectionID).FirstOrDefault();
                return (subsectionId == 0) ? null : subsectionId;
            }
        }
        public int? getPaymentAuthorisationSubSectionIDIfPresentInSection(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                return db.BQCSubSections.Where(r => r.BQCSectionID == SectionID && r.SubSectionTypeEnum == (int)SubSectionType.PaymentAuthorisation).Select(r => r.BQCSubSectionID).FirstOrDefault();
            }
        }
        public bool hasWorkerSubSectionBeenCompletedAndApproved(int SubSectionID, int PlotID)
        {
            int status = getStatusOfTradeContractorSubSection(SubSectionID, PlotID);
            bool hasWorkerSubSectionBeenCompletedAndApproved = (status == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || status == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
            return hasWorkerSubSectionBeenCompletedAndApproved;
        }
        public bool isSectionCompleted(int sectionId, int plotId)
        {
            int? tradeContractorSubSectionID = getTradeContractorSubSectionIDIfPresentInSection(sectionId);
            if (tradeContractorSubSectionID != null)
            {
                int statusOfTradeSubSection = getStatusOfTradeContractorSubSection((int)tradeContractorSubSectionID, plotId);
                bool tradeSubSectionCompleted = (statusOfTradeSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || statusOfTradeSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                return (tradeSubSectionCompleted);
            }
            int? staffSubSectionID = getStaffSubSectionIDIfPresentInSection(sectionId);
            if (staffSubSectionID != null)
            {
                int statusOfStaffSubSection = getStatusOfStaffSubSection((int)staffSubSectionID, plotId);
                return (statusOfStaffSubSection == (int)StatusOfStaffSubSection.Complete);
            }
            return false;
        }
        public int getStatusOfSection(int SectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                if (!checkDependentSectionsAndSubSectionsAreCompleted(SectionID, PlotID)){
                    return (int)StatusOfSections.DependenciesIncomplete;
                }
                return (isSectionCompleted(SectionID, PlotID) ? (int)StatusOfStaffSubSection.Complete : (int)StatusOfStaffSubSection.Incomplete);
            }
        }
        public int getStatusOfSubSection(int RoleEnumValue, int SubSectionID, int PlotID)
        {
            //rolenumvalue is redundant for this code
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                BQCSubSections subSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).FirstOrDefault();
                int subSectionType = getSubSectionTypeEnum(subSection, null);
                switch (subSectionType)
                {
                    case (int)SubSectionType.PaymentAuthorisation:
                        return getStatusOfPaymentAuthorisationSection(SubSectionID, PlotID);
                        break;
                    case (int)SubSectionType.StaffChecklist:
                        return getStatusOfStaffSubSection(SubSectionID, PlotID);
                        break;
                    case (int)SubSectionType.TradeContractorChecklist:
                        return getStatusOfTradeContractorSubSection(SubSectionID, PlotID);
                        break;
                    default:
                        return 0;
                }
            }
        }
        public int getStatusOfStaffSubSection(int SubSectionID, int PlotID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int sectionID = db.BQCSubSections.Where(r=>r.BQCSubSectionID == SubSectionID).Select(r=>r.BQCSectionID).FirstOrDefault();
                var questionResponseInfo = getSubSectionQuestionsResponseInformation(PlotID, SubSectionID);
                //bool priorSubSectionsCompleted = checkDependentSubSectionIsCompleted(sectionID, PlotID);
                //if (!priorSubSectionsCompleted)
                //{
                //    return (int)StatusOfStaffSubSection.SubSectionDependenciesIncompleted;
                //}
                if (questionResponseInfo.TotalQuestionsAttempted == questionResponseInfo.TotalQuestionsInSubSection)
                {
                    return (int)StatusOfStaffSubSection.Complete;
                }
                else
                {
                    return (int)StatusOfStaffSubSection.Incomplete;
                }
            }
        }
        public int getSectionIDFromSubSectionID(int SubSectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                return db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.BQCSectionID).FirstOrDefault();
            }
        }
        public SubSectionQuestionsResponseInformationModel getSubSectionQuestionsResponseInformation(int PlotID, int SubSectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                List<int> allQuestionIDsInSubSection = db.BQCQuestions.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.BQCQuestionID).ToList();
                int totalQuestionsInSubSection = allQuestionIDsInSubSection.Count();
                int totalQuestionsApproved = 0;
                int totalQuestionsRejected = 0;
                int totalQuestionsMarkedAsConditional = 0;
                int totalQuestionsResponded = 0;
                int totalQuestionsAttempted = 0;
                foreach (int questionID in allQuestionIDsInSubSection)
                {
                    int? approverResponseEnum = db.BQCResponses.Where(r => r.BQCQuestionID == questionID && r.PlotID == PlotID).OrderByDescending(r => r.AcceptorWorkCompletedDateTime).Select(r => r.ApproverResponseEnumValue).FirstOrDefault();
                    bool acceptedByTradeContractorOrStaff = db.BQCResponses.Where(r => r.BQCQuestionID == questionID && r.PlotID == PlotID).OrderByDescending(r => r.AcceptorWorkCompletedDateTime).Select(r => r.AcceptorWorkCompleted).FirstOrDefault();
                    if (approverResponseEnum == (int)ApprovalResponseTypes.Approve)
                    {
                        totalQuestionsApproved++;
                    }
                    if (approverResponseEnum == (int)ApprovalResponseTypes.Reject)
                    {
                        totalQuestionsRejected++;
                    }
                    if (approverResponseEnum == (int)ApprovalResponseTypes.SubjectToConditions)
                    {
                        totalQuestionsMarkedAsConditional ++;
                    }
                    if (acceptedByTradeContractorOrStaff)
                    {
                        totalQuestionsAttempted++;
                    }
                    if (approverResponseEnum != (int)ApprovalResponseTypes.Incomplete)
                    {
                        totalQuestionsResponded++;
                    }
                }

                return new SubSectionQuestionsResponseInformationModel()
                {
                    TotalQuestionsInSubSection = totalQuestionsInSubSection,
                    TotalQuestionsAttempted = totalQuestionsAttempted,
                    TotalQuestionsApproved = totalQuestionsApproved,
                    TotalQuestionsMarkedAsConditional = totalQuestionsMarkedAsConditional,
                    TotalQuestionsRejected = totalQuestionsRejected,
                    TotalQuestionsResponded = totalQuestionsResponded,
                };
            }
        }

        public string getRoleNameEnumOfAppropriateSubSectionInSection(int SectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int? tradeContractorSubSectionID = getTradeContractorSubSectionIDIfPresentInSection(SectionID);
                if (tradeContractorSubSectionID != null && tradeContractorSubSectionID != 0)
                {
                    int roleEnumValue = (int)db.BQCSubSections.Where(r=>r.BQCSubSectionID==tradeContractorSubSectionID).Select(r=>r.RoleNameEnum).FirstOrDefault();
                    return ((AllRoles) roleEnumValue).AsString(EnumFormat.Description);
                }
                else
                {
                    int? siteSubSectionID = getSiteSubSectionIDFromSectionID(SectionID);
                    if (siteSubSectionID != null && siteSubSectionID != 0)
                    {
                        return (AllRoles.SiteManagement).AsString(EnumFormat.Description);
                    }
                    else
                    {
                        return "Not found.";
                    }
                }
            }
        }
        #endregion Checklist



        #region Saving
        public void saveAcceptorChecklistResponses(SaveSubSectionModel saveSubSectionModel)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                bool anythingSaved = false;
                foreach (SaveChecklistQuestionModel checklistQuestion in saveSubSectionModel.ChecklistQuestionResponses)
                {
                    bool proceedWithSave = false;

                    //check if already been approved
                    BQCResponses responseAlreadySaved = db.BQCResponses.Where(r => r.PlotID == saveSubSectionModel.PlotID && r.BQCQuestionID == checklistQuestion.QuestionID).OrderByDescending(r => r.ApproverResponseDateTime).FirstOrDefault();
                    if (responseAlreadySaved != null)
                    {
                        if (responseAlreadySaved.ApproverResponseEnumValue != (int)ApprovalResponseTypes.Approve)
                        {
                            //if it has not already been approved then save it 
                            proceedWithSave = true;
                        }
                    }
                    else
                    {
                        //if there is no current record then save it 
                        proceedWithSave = true;
                    }

                    if (proceedWithSave)
                    {
                        BQCResponses response = new BQCResponses()
                        {
                            PlotID = saveSubSectionModel.PlotID,
                            BQCQuestionID = checklistQuestion.QuestionID,
                            AcceptorUserID = currentUserID,
                            AcceptorName = saveSubSectionModel.PrintName,
                            AcceptorWorkCompleted = true,
                            AcceptorWorkCompletedComment = checklistQuestion.Note,
                            AcceptorWorkCompletedDateTime = (DateTime)DateTime.Now,
                            ApproverResponseDateTime = null,
                        };

                        db.BQCResponses.Add(response);
                        db.SaveChanges();

                        anythingSaved = true;
                    }
                }

                if (anythingSaved)
                {
                    saveSignature(saveSubSectionModel.PlotID, saveSubSectionModel.SubSectionID, saveSubSectionModel.Signature, saveSubSectionModel.PrintName);
                    SendEmailIfAppropriateAfterSubSectionCompletion(saveSubSectionModel.PlotID, saveSubSectionModel.SubSectionID);
                }
            }
        }
        public void saveSignature(int PlotID, int SubSectionID, byte[] SignatureData, string SignatureName)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int roleNameEnum = (int)db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r => r.RoleNameEnum).FirstOrDefault();
                BQCSubSectionSignatures signature = new BQCSubSectionSignatures()
                {
                    RoleNameEnum = roleNameEnum,
                    PlotID = PlotID,
                    BQCSubSectionID = SubSectionID,
                    UserID = currentUserID,
                    Signature = SignatureData,
                    Name = SignatureName,
                    SignedDateTime = DateTime.Now,
                };
                db.BQCSubSectionSignatures.Add(signature);
                db.SaveChanges();
            }
        }
        public void savePaymentAuthorisation(SaveSubSectionModel saveSubSectionModel)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                saveSignature(saveSubSectionModel.PlotID, saveSubSectionModel.SubSectionID, saveSubSectionModel.Signature, saveSubSectionModel.PrintName);
            }
        }
        public void saveApprovalChecklistResponses(SaveSubSectionModel saveSubSectionModel)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                bool anythingSaved = false;
                foreach (SaveChecklistQuestionModel checklistQuestion in saveSubSectionModel.ChecklistQuestionResponses)
                {
                    bool saveRecord = false;

                    //check for response 
                    var record = db.BQCResponses.Where(r => r.PlotID == saveSubSectionModel.PlotID && r.BQCQuestionID == checklistQuestion.QuestionID).OrderByDescending(r => r.AcceptorWorkCompletedDateTime).FirstOrDefault();

                    if (record != null)
                    {
                        if (record.ApproverResponseEnumValue != (int)ApprovalResponseTypes.Approve)
                        {
                            saveRecord = anythingSaved = true;
                        }
                    }
                    else
                    {
                        saveRecord = anythingSaved = true;
                    }
                    if (saveRecord)
                    {
                        record.ApproverUserID = identityDictionary.getUserIDOfCurrentUser();
                        record.ApproverResponseComment = checklistQuestion.Note;
                        record.ApproverResponseDateTime = DateTime.Now;
                        record.ApproverResponseEnumValue = checklistQuestion.ApproverResponseEnum;

                        db.SaveChanges();
                    }
                }

                if (anythingSaved)
                {
                    saveSignature(saveSubSectionModel.PlotID, saveSubSectionModel.SubSectionID, saveSubSectionModel.Signature, saveSubSectionModel.PrintName);
                    SendEmailIfAppropriateAfterSubSectionCompletion(saveSubSectionModel.PlotID, saveSubSectionModel.SubSectionID);
                }

                //check if all approved for email
                bool allApproved = true;
                List<BQCQuestions> allQuestions = db.BQCQuestions.Where(r => r.BQCSubSectionID == saveSubSectionModel.SubSectionID).ToList();
                foreach (BQCQuestions question in allQuestions)
                {
                    List<BQCResponses> allResponses = db.BQCResponses.Where(r => r.BQCQuestionID == question.BQCQuestionID && r.PlotID == saveSubSectionModel.PlotID).OrderByDescending(r => r.AcceptorWorkCompletedDateTime).ToList();
                    foreach (BQCResponses response in allResponses)
                    {
                        if (response.ApproverResponseEnumValue != (int)ApprovalResponseTypes.Approve)
                        {
                            allApproved = false;
                        }
                    }
                }

            }
        }
        #endregion Saving



        #region Emails
        public EmailSendingInfoModel getEmailSendingInfo(int SubSectionID, int PlotID)
        {
            using(SiteManagementSystemContext db = new SiteManagementSystemContext())
            {
                int roleEnumValue = (int)db.BQCSubSections.Where(r => r.BQCSubSectionID == SubSectionID).Select(r=>r.RoleNameEnum).FirstOrDefault();
                int sectionID = getSectionIDFromSubSectionID(SubSectionID);
                int siteID = getSiteIDFromPlotID(PlotID);
                string link = "https://bqc.croudace.co.uk/Checklist/LoadChecklist?plotId=" + Utils.Encrypt(PlotID.ToString()) + "&SectionID=" + sectionID;
                return new EmailSendingInfoModel()
                {
                    Link = link,
                    SiteEmails = getSiteOfficeEmailInSite(siteID),
                    TradeContractorEmails = getTradeContractorEmailsInSiteAndRole(siteID, roleEnumValue),
                    PlotID = PlotID,
                    PlotName = getPlotNameFromPlotID(PlotID),
                    SiteID = siteID,
                    SiteName = getSiteNameFromSiteID(siteID),
                    SubSectionID = SubSectionID,
                    SubSectionName = getSectionFullNameFromSectionID(sectionID),
                };
            }
        }
        public void SendEmailIfAppropriateAfterSubSectionCompletion(int PlotID, int CompletedSubSectionID)
        {
            using (SiteManagementSystemContext db = new SiteManagementSystemContext()) {
                BQCSubSections subSection = db.BQCSubSections.Where(r => r.BQCSubSectionID == CompletedSubSectionID).FirstOrDefault();
                BQCSections section = db.BQCSections.Where(r => r.BQCSectionID == subSection.BQCSectionID).FirstOrDefault();
                int subSectionType = getSubSectionTypeEnum(subSection, null);
                if (subSectionType == (int)SubSectionType.StaffChecklist)
                {
                    int? tradeContractorSubSectionID = getTradeContractorSubSectionIDIfPresentInSection(subSection.BQCSectionID);
                    if (tradeContractorSubSectionID != null)
                    {
                        SendTradeContractorCompletionNeededEmail((int)tradeContractorSubSectionID, PlotID);
                    }
                    else
                    {
                        sendEmailsForNextSectionCompletion(section.SortOrderValue + 1);
                    }
                }
                if (subSectionType == (int)SubSectionType.TradeContractorChecklist)
                {
                    int tradeContractorSubSectionStatus = getStatusOfTradeContractorSubSection(CompletedSubSectionID, PlotID);
                    bool subSectionCompletedAndApproved = (tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                    bool awaitingStaffResponse = (tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.AwaitingResponseFromStaff);
                    bool awaitingResponseFromTradeContractor = (tradeContractorSubSectionStatus == (int)StatusOfTradeContractorSubSection.AmendmentByTradeContractorNeeded);
                    if (subSectionCompletedAndApproved)
                    {
                        sendEmailsForNextSectionCompletion(section.SortOrderValue + 1);
                    }
                    //else if (awaitingStaffResponse)
                    //{
                    //    SendSiteStaffTradeContractorSubSectionResponseNeededEmail(CompletedSubSectionID, PlotID);
                    //}                    
                    else if (awaitingResponseFromTradeContractor)
                    {
                        SendTradeContractorSubSectionAmendmentNeededEmail(CompletedSubSectionID, PlotID );
                    }
                }

                void sendEmailsForNextSectionCompletion(int nextSectionNumber){
                    BQCSections nextSection = db.BQCSections.Where(r => r.SortOrderValue == nextSectionNumber).FirstOrDefault();
                    if (nextSection != null) //not the final subsection
                    {
                        //if there is site subsection then email site staff to complete their checklist, otherwise email subbie to complete theirs
                        int? siteStaffSubSectionIDInNextSection = getSiteSubSectionIDFromSectionID(nextSection.BQCSectionID);
                        if (siteStaffSubSectionIDInNextSection != null)
                        {
                            //SendSiteStaffSubSectionCompletionNeededEmail((int)siteStaffSubSectionIDInNextSection, PlotID);
                        }
                        else
                        {
                            int? tradeContractorSubSectionIDInNextSection = getTradeContractorSubSectionIDIfPresentInSection(nextSection.BQCSectionID);
                            if (tradeContractorSubSectionIDInNextSection != null)
                            {
                                int statusOfTradeContractorSubSection = getStatusOfTradeContractorSubSection((int)tradeContractorSubSectionIDInNextSection, PlotID);
                                bool tradeContractorSectionApproved = (statusOfTradeContractorSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffAndPaymentAuthorised || statusOfTradeContractorSubSection == (int)StatusOfTradeContractorSubSection.CompletedAndApprovedByStaffButPaymentNotAuthorised);
                                if (tradeContractorSectionApproved)
                                {
                                    SendTradeContractorCompletionNeededEmail((int)tradeContractorSubSectionIDInNextSection, PlotID);
                                }
                            }
                        }
                    }
                }
            }
        }
        public string GetEmailImageString()
        {
            return $"<br><br><img src=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANAAAAA0CAYAAAD7cbbgAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAArQSURBVHhe7Zkxj11JEYXn9/AL/BMI/QuIHBIQOCBEwjFIzgiQLCFZQjIpOAQ52QQLOcGEOEJotFqB1iwIePAN7zxqek5V973v7Y7b7iMd+fl2d3V1VZ3uvneuDgsLC7uxBLSwcAaGBfT27dvDq1evDi9evLjhy5cvb569f//+2GOhhWLV8vr6+tjj08Wfr747NYVSQCT66dOnhwcPHhyurq5SPnr06EZcC7fhYgXZeD51uKKciUIqIHZKl/yKCGntrv+HixFcAvrIBcSp4xI/Qk6rda37H1x84BLQRyygZ8+e2aRv4ePHj4/WPm242MAloI9UQCTWJXwPV5EsAVVwRTkThVsC4uRwCRcfPnx4+pLE+47rIz558uRodRxb3p8+tCJ0vru4wFHf3717d/y1H/h1iffSS/gS4YpyJgonAREgl2yR96IWvXclAWE6MifJ5Xcch0AjeKfis3nbT0TMXD2zQolzRrovh64fbAsIn9wXSjYZ2Y3PI1sBaX1sOq6/iB/0q94xiUG2weEbPo8ImDVk/mCbOc4RpivKmSicqpyAuGBBAp+BNjcGKlGuDb5+/domOgqIPr3P6JEUWAvXD7ZCBa4fjEVHAfdO4GpzibYQ5pb1QWLuipeiH7WFf06I+NNbm8hcLoYjcEU5E4WTgNjdXJAgu3sGkhZ36kjt2s4mzBKlpCAe195jm1TXB+4R0Ih4epStveuDCCBiz8cfchSxR8yw9WUErihnonAS0MhJshfOZkUKmx12TzLF6LNrh3sExBjXvoWydY4QiY1OkHOEGK+x5/iDD1vginImCicBuaCI58LZrEiRVlcgyO5ZCSzurq4d7hFQtdHQ1vMLypZrg/g1Io5Rn6p2RAMQkmtnLfjDXPTJRMYcW+CKciYKH4SACD5XEBLFiyv/ZkVIAuPdvRJa7wq5VUDQtcH2GlPt5tjRy75jXB+/nQ2InUpg8X2Q364PZI5MZMzRIuureI/AFeVMFO5dQO7+XBVqe1WoCkwF5NogxdrC9YP41CvCCATi+kFXlBG0Q/zjRHM2IH2ydx93IrA5Ya/lmzdvrA02MfkSmW1aLp4ZXFHOROGkjmxXgQTtHDibkAS1hQeqQnW7HEXg+iqhrg1uFRD9XRvzO2SnaBtPNgWKssqBI3aytWc+OWDH2djKLX/7c0U5E4WTgLJEwHgVaFF9/ekVsDt9QFao0OGbElD2d5GsWDO/JCA2j8zmCCsBubVluJSAsjg4uKKcicKpIquiddcBodo1dd1ybTBLcuWLQ+aD7Ls22M7fu3Zlm0VWOJlf2Br9yoiN6iTLBLTlNFgC2k7hVJFcjVxQRFfs1VULUiTAtcFMQFVC2ytc9Q7UE1BbZL0vXthzbRR4i8ovbGXvEZBCZC6tNRNJZUdf1yLoi62Wz58/tzYgMWGeEX7SHxEAwXRBFGlHNHzOrAoAxgS6dpgJqCq+tugrEZNU4Nogha+/g9C3Ok1prwTWXnOr+GArizXPI3qnYrX+WNA9O9kpF/9OdEm4opyJwi0BEUgXxD3U9Q24dpgJCFTvBhQZY6siRQzCyFWpR2LTsxU/xbt2EVuZWKPfAHuuH8ROtdngKz4Rp8xvnoMslrQjUuYB/M5sbRGbK8qZKNx5qag+CoyyPSVcH1gJ6Fwxx2T2TtYRSkBVQY8SW5VPiIiCrk5EeAmflIPqhIKZaMRW+D24opyJgn0rr3b2Hts/dALXD1YCAnsLoxUwYnL9tlDFytp6hd0jti4hxHjKV3+4zdi+J+2NE+La8v4DXFHORMEKCOw5idhVW/EA1xf2BAS2+pH5MHIKVX0kIECx9Hbkqh1b+DhS9NjJBEtsBOxt2fiyOCGi3toi94gHuKKciUIqIECie/d5SIKr+68bA0cEBPCjJ4CeDxRLJUYVo2uDUUCAosl84nnvpR30ip42+mTzuGsTMahOyF6cAL4zdyUk2sifE+EIXFHORKEUkECQSDoBiyQRe3afvZAfvMjKB/5PwkeBDa4+cfzeIgCsnzjs8UVgTPSJ33vsRDibe3LFmrQ+eKmcu6KcicKQgBYWLg1XlDNRWAJauBe4opyJwhLQwr3AFeVMFJaAFu4FrihnorAEtHAvcEU5E4UloIV7gSvKmSgsAS3cC1xRzkRhCWjhXuCKciYKJwF98b2f3PCf7/50fHIXX/70Vzd9vvrN745PbuPf7786/PXHvzh8/u0fnia6/tb3D3/5wc9Ku5qbsT3Ih9ZXfut5Rca3+Mfv/3jjI77KZ/pm6xzB33/7hxsbMQ6jNtux8PPv/Ojw5c9/fewxDubL1h1BH+hiWo2V/XZsD3FtM1I4CUgNFFMGgkQfl0iCF4UDVZDi33752bH3bcQ+/7r+4vj0LhBo7Bt95Xdsy8gaIhBtbG/XgLC2goKLNlqbiIG1OLRj2xgytopRC3LFuHbdLWTfxTQbG2O3dbPRuFkpXERAiEfjSXC0QbJjUTjxqe2mvdjtEGDs65Kt5xnjLslOrzGxAPCZuVS8mfAdYlExLgqF+WWTOLVox0ah4KuEiI1MgC2+LgGdIx6gsbNSuIiA9JzdOkusit8lX3NDiiQDRScb/OuSDUchYVMMDiq+0VMobiTZdSbrE/3PckDcFAO3ETl8HQI6VzxA42elcLaAYkH0rhbaQdvkazyFyr/sti00DwUkP1yy4ShUCJmAmJO5RgUk/zN7AuvHbiy+0bGM0TpHrnKXFtAlxANkY1YKZwtICRopMiW/Tabm1pXKFZFOC04y+XGugHQqQtYxUpAVZKuKYYYtY7URjfS9pIAkHm4A8fTcA803K4U7AiJIBN3RnSAq7PgsgxJCAiI0N9D1rL3mxec9AUWfW0aRxCuRyP+JAWJvfaiAXdnYMg7QX2NHoPWzAfTAmul7roDiyUOMzoVszUrhjoBGSFIEJTM+yxCLLCI+kyDj9UAnk065noAqxjECc8lmSwpnRBBx/q3YOlbFPBLzSwhI1CYGR+auEO3OSOGOgNjVCJyje4G99AkU33UEvR/o3agnIPnr2BMDfVhLPJlGdtz4LvixnUCQnLHGeO2NfbdCNmal8EG9AwkqXk4sFVcUXU9Ao2BMtV6dfO1cGUb7uqLeMs83/Q4k8QiKP8+3bhaC5puVwtkCijtv7yW89xVO0C7H6RZ/C5cSkPpXReBO3QzyiytWBZ2o0aae9cZqE4K9eANtAtUpGk/AGAvFtBUffRCPaxuF5puVwtkCAnpOEWTFKCG4XUtzC0oogpPo3A54roAkjuoqpPl1fawQfYj+RmR94vO4rgjiskXQIG5wmU8xNxHyyYkk+lvFL4PGzkrhIgKKSSLB0Qa7pN6T3FigtgjtyLDdPXsC4ndFQYUD8SsKm996WW9FrzGwhXyD2I/jEGG1c2s+jY0nDGMl5moTgi3kU5sbwIkmn9rc0JfnzlcQ85qJM4PGzUrhIgICBFAJFpUYMdup1B4R3z/acfIj+qpkjzAiFi2Mpx5kDW1xxP4tKOwoftjGhUJuBSDEooRtDBnrrm6xTwv6Rzv8JobxmfOpJyDAuJvx/11jtiYHzTsrhZOACBKsdhKSS5/4iTmCAFKQbQFSUJVdzd1Cz9vEyI9ok9/q32MLCgUfo98UBvNsLVaBGKm4RP6fbSIRbB742Y51G5cQ+zoQQ8ZjtxVO5pNiShwyxLhndeEQ/Z2RwklACwsL27EEtLCwG4fDfwB2OG8w7XgBAwAAAABJRU5ErkJggg==/><br><br>";
        }
        public string GetEmailClickHereButton(string ButtonText, string ButtonLink)
        {
            return "</br></br/><a href='" + ButtonLink + "'>"+ButtonText+"</a>";
        }
        public void SendSiteStaffTradeContractorSubSectionResponseNeededEmail(int SubSectionID, int PlotID)
        {
            //EmailSendingInfoModel emailSendingInfo = getEmailSendingInfo(SubSectionID, PlotID);
            //string siteBody = "<h2>Build Quality Checklist Response Needed</h2>"+
            //    "<p>The Trade Contractor has completed their checklist for Section: "+ emailSendingInfo.SubSectionName+" in Site: "+emailSendingInfo.SiteName+" and Plot: "+emailSendingInfo.PlotName+ ".</p>" +
            //    "<br/>Your approval is needed. "+
            //    GetEmailClickHereButton("Click here to approve", emailSendingInfo.Link) +
            //    GetEmailImageString();
            //SMTPEmail.SendEmail(emailSendingInfo.SiteEmails, "Build Quality Checklist Approval Needed", siteBody);
        }
        public void SendSiteStaffSubSectionCompletionNeededEmail(int SubSectionID, int PlotID)
        {
            //EmailSendingInfoModel emailSendingInfo = getEmailSendingInfo(SubSectionID, PlotID);
            //string siteBody = "<h2>Build Quality Checklist Completion Needed</h2>" +
            //                  "<p>The checklist for Section: " + emailSendingInfo.SubSectionName + " in Site: " + emailSendingInfo.SiteName + " and Plot: " + emailSendingInfo.PlotName + " is awaiting your completion.</p>" +
            //                  GetEmailClickHereButton("Click here to respond", emailSendingInfo.Link) +
            //                  GetEmailImageString();
            //SMTPEmail.SendEmail(emailSendingInfo.TradeContractorEmails, "Build Quality Checklist Sub Section Completion", siteBody);
        }
        public void SendTradeContractorSubSectionAmendmentNeededEmail(int SubSectionID, int PlotID)
        {
            EmailSendingInfoModel emailSendingInfo = getEmailSendingInfo(SubSectionID, PlotID);
            string siteBody = "<h2>Build Quality Checklist Response Needed</h2>" +
                "<p>Site Staff have not approved your checklist for Section: " + emailSendingInfo.SubSectionName + " in Site: " + emailSendingInfo.SiteName + " and Plot: " + emailSendingInfo.PlotName + ".</p>" +
                GetEmailClickHereButton("Click here to respond", emailSendingInfo.Link) +
                GetEmailImageString();
            SMTPEmail.SendEmail(emailSendingInfo.TradeContractorEmails, "Build Quality Checklist Section Completion", siteBody);
        }
        public void SendTradeContractorCompletionNeededEmail(int SubSectionID, int PlotID)
        {
            EmailSendingInfoModel emailSendingInfo = getEmailSendingInfo(SubSectionID, PlotID);
            string siteBody = "<h2>Build Quality Checklist Completion Needed</h2>" +
                "<p>The Build Quality Checklist for Section: " + emailSendingInfo.SubSectionName + " in Site: " + emailSendingInfo.SiteName + " and Plot: " + emailSendingInfo.PlotName + " is ready to be completed." +
                GetEmailClickHereButton("Click here to complete", emailSendingInfo.Link) +
                GetEmailImageString();
            SMTPEmail.SendEmail(emailSendingInfo.TradeContractorEmails, "Build Quality Checklist Ready to Complete", siteBody);
        }
        #endregion Emails
    }
}