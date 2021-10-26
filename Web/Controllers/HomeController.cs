using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CharacterBuilder.ChampionsOnline.Model;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Summary()
        {
            return View();
        }

        public JsonResult BuildData()
        {
            Statistics stats = Statistics.Deserialize(Server.MapPath(@"/App_Data/data.xml"));
            JsonResult json = new JsonResult();
            json.Data = stats;
            return json;
        }
    }
}