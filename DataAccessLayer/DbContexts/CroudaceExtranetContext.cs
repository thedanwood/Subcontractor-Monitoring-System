using BQC.DataAccessLayer.Entities;
using SiteManagementSystemDomain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BQC.DataAccessLayer
{
    public class CroudaceExtranetContext : DbContext
    {
        public DbSet<ThirdPartyUser> ThirdPartyUsers { get; set; }
        public DbSet<ThirdPartySiteAccess> ThirdPartySiteAccess { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
    }
}