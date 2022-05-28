using BQC.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Web.Security;
using BQC.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using CroudaceDomainLibrary.Concrete;
using BQC.Enums;

namespace BQC.DataAccessLayer.Dictionaries
{
    public class IdentityDictionary
    {
        UserStore<IdentityUser> userStore;
        UserManager<IdentityUser> userManager;
        public IdentityDictionary()
        {
            userStore = new UserStore<IdentityUser>();
            userManager = new UserManager<IdentityUser>(userStore);
        }



        public string getFullNameOfCurrentUser()
        {
            string username = getUsernameOfCurrentUser();
            return getFullNameOfUserFromUsername(username);
        }

        public string getFullNameOfUserFromStaffUserID(string userID)
        {
            using (CroudaceExtranetContext db = new CroudaceExtranetContext())
            {
                string username = db.AspNetUsers.Where(r => r.Id == userID).Select(r => r.UserName).FirstOrDefault();
                return getFullNameOfUserFromUsername(username);
            }
        }

        public string getFullNameOfUserFromUsername(string username)
        {
            using (CroudaceExtranetContext db = new CroudaceExtranetContext())
            {
                if (HttpContext.Current.User.IsInRole("Subcontractor"))
                {
                    return "";
                }
                else
                {
                    int? contactID = getContactIDOfUser(username);
                    if (contactID == null)
                    {
                        return "";
                    }
                    else
                    {
                        using (CroudaceContext croudaceDb = new CroudaceContext())
                        {
                            string firstLetterOfFirstName = croudaceDb.Contacts.Where(r => r.ContactID == contactID).Select(r => r.FirstName).FirstOrDefault().Substring(0, 1);
                            string lastName = croudaceDb.Contacts.Where(r => r.ContactID == contactID).Select(r => r.Surname).FirstOrDefault();
                            return firstLetterOfFirstName + " " + lastName;
                        }
                    }
                }
            }
        }
        public int? getContactIDOfUser(string Username)
        {
            using(CroudaceContext db = new CroudaceContext())
            {
                return db.Users.Where(r => r.UserName == Username).Select(r => r.ContactID).FirstOrDefault();
            }
        }
        public int? getContactIDOfCurrentUser()
        {
            string username = getUsernameOfCurrentUser();
            return getContactIDOfUser(username);
        }
        public string getUsernameOfCurrentUser()
        {
            return HttpContext.Current.User.Identity.Name;
        }
        public string getUserIDOfCurrentUser()
        {
            using (CroudaceExtranetContext db = new CroudaceExtranetContext())
            {
                string username = getUsernameOfCurrentUser();
                return (username != null && username != "") ? db.AspNetUsers.Where(r => r.UserName == username).Select(r => r.Id).FirstOrDefault() : null;
            }
            
        }
        public string getAccessRightOfCurrentUser()
        {
            string username = getUsernameOfCurrentUser();
            return userManager.GetRoles(username).Single();
            //string userID = getUserIDOfCurrentUser();
            //return UserManager.GetRoles(userID)[0];
        }
        public int getRoleEnumValueOfCurrentUser()
        {
            using (CroudaceExtranetContext db = new CroudaceExtranetContext())
            {
                string userID = getUserIDOfCurrentUser();
                if (userID != null && userID != "")
                {
                    var thirdPartyUser = db.ThirdPartyUsers.Where(r => r.UserId == userID).FirstOrDefault();
                    return (thirdPartyUser == null) ? (int)AllRoles.SiteManagement : (int)thirdPartyUser.TradeTypeId;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}