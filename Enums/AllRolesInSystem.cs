using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BQC.Enums
{
    public enum AllRoles
    {
        [Description("Site Management")]
        SiteManagement=1,
        [Description("Groundworker")]
        Groundworker =2,
        [Description("Bricklayer")]
        Bricklayer =3,
        [Description("Carpenter")]
        Carpenter =4,
        [Description("Roofer")]
        Roofer =5,
        [Description("Electrician")]
        Electrician = 6,
        [Description("Dryliner")]
        Dryliner =7,
        [Description("Painter")]
        Painter =8,
        [Description("Plumber")]
        Plumber = 9,
        [Description("Floor and Stairs Precaster")]
        PrecastFloorAndStairs = 10,
        [Description("PV Contractor")]
        PVContractor = 11,
        [Description("Ceramic Tiler")]
        CeramicTiler = 12,
        [Description("Loft Insulator")]
        LoftInsulator = 13,
        [Description("White Goods")]
        WhiteGoodContractor = 14,
        [Description("Kitchen Contractor")]
        KitchenContractor = 15,
        [Description("Mastic Contractor")]
        MasticContractor = 16,
        [Description("Cleaner")]
        Cleaner = 17,
        [Description("Fencer")]
        Fencer = 18,
        [Description("Metal Railer")]
        MetailRailer = 19,
        [Description("Carport")]
        CarportContractor = 20,
        [Description("Metal Gater")]
        MetailGatingContractor = 21,
        [Description("Juliettes Contractor")]
        JulietteContractor = 22,
        [Description("Glazed Canopies")]
        GlazedCanopyContractor = 23,
        [Description("Shower Screen")]
        ShowerSreenContractor = 24,
        [Description("Metal Balcony")]
        MetalBalconyContractor = 25,
        [Description("Windows Contractor")]
        WindowsContractor = 26,
        [Description("Fire Surround")]
        FireSurroundContractor = 27,
        [Description("Granite Worktops")]
        GraniteWorktops = 28,
        [Description("Security Alarms")]
        SecurityAlarms = 29,
        [Description("Floorer")]
        FlooringContractor = 30,
        [Description("Landscaping")]
        Landscaper = 31,
    }
}