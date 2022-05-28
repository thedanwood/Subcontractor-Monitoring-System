using BuildQualityDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.DataAccessLayer.Dictionaries
{
    public class CroudaceDictionary
    {
        CroudaceContext db = new CroudaceContext();

        public List<Site> GetActiveSites()
        {
            
            List<Site> result =  db.Sites.Where(m => m.Status == 1 && m.SubStatusID < 4).ToList();

            return result;
        }
    }
}