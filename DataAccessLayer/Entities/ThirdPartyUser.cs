using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BQC.DataAccessLayer.Entities
{
    public class ThirdPartyUser
    {
        [Key]
        public int ThirdPartyUserId { get; set; }
        public string UserId { get; set; }
        public int CroudaceContactId { get; set; }
        public string CompanyName { get; set; }
        public int TradeTypeId { get; set; }

        public bool Active { get; set; }
    }
}