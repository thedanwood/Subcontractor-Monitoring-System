using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BQC.Models.Identity
{
    public class UserAccessRightInfo
    {
        //access right value corresponds to aspnet roles value assigned to user in aspnet users table
        public int UserAccessRightValue { get; set; }
    }
}