using BuildQualityDomain.Models;
using CroudaceDomainLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BQC.DataAccessLayer
{
    public class HouseTypesContext : DbContext
    {
        public DbSet<SiteBaseHouseType> SiteBaseHouseTypes { get; set; }
        //public DbSet<PlotHouseTypes> PlotHouseTypes { get; set; }
    }
}