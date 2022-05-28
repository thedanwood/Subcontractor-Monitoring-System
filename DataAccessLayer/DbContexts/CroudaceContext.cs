using BuildQualityDomain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.DataAccessLayer
{
    public class CroudaceContext : DbContext
    {
        public DbSet<Site> Sites { get; set; }
        public DbSet<SitePlot> SitePlots { get; set; }
        public DbSet<SiteNumber> SiteNumbers { get; set; }
    }
}