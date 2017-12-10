using BadBee.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BadBeeCatalog.Controllers
{
    public class ReloadEE0B4864Controller : Controller
    {
        // GET: ReloadEE0B4864
        public ActionResult Index()
        {
            GlobalVars.DictionaryCache = new Dictionary<Type, object>();
            BadBee.Core.Providers.ListProvider.FillDictionaryCache();

            return RedirectToAction("Index", "Search");
        }
    }
}
