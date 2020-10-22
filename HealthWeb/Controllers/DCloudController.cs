using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthWeb.Controllers
{
    public class DCloudController : Controller
    {
        // GET: DCloud
        public ActionResult Index()
        {
            return View("DCloudTraffic");
        }
        public ActionResult DCloudTraffic()
        {
            return View();
        }
        public ActionResult EquipmentTraffic()
        {
            return View();
        }
        public ActionResult PlacementTraffic()
        {
            return View();
        }
    }
}