using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BQC.DataAccessLayer.Entities
{
    [Table("ThirdPartySiteAccess")]
    public class ThirdPartySiteAccess
    {
        [Key]
        public int ThirdPartySiteId { get; set; }
        public string UserId { get; set; }
        public int SiteId { get; set; }
    }
}