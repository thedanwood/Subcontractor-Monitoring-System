using SiteManagementSystemDomain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BQC.DataAccessLayer
{
    public class SiteManagementSystemContext : DbContext
    {
        public DbSet<BQCFiles> BQCFiles { get; set; }
        public DbSet<BQCQuestions> BQCQuestions { get; set; }
        public DbSet<BQCResponses> BQCResponses { get; set; }
        public DbSet<BQCSections> BQCSections { get; set; }
        public DbSet<BQCSectionDependencies> BQCSectionDependencies { get; set; }
        public DbSet<BQCSubSections> BQCSubSections { get; set; }
        //public DbSet<BuildQualityChecklistSubSections> BuildQualityChecklistSubSections { get; set; }
        public DbSet<BQCSubSectionSignatures> BQCSubSectionSignatures { get; set; }
        public DbSet<BQCPlotSectionStartingPoints> BQCPlotSectionStartingPoints { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<PermitChecklistQuestions> PermitChecklistQuestions { get; set; }
        public DbSet<PermitChecklistResponses> PermitChecklistResponses { get; set; }
        public DbSet<Permits> Permits { get; set; }
        public DbSet<PermitSignatures> PermitSignatures { get; set; }
        public DbSet<PermitEdits> PermitEdits { get; set; }
    }
}