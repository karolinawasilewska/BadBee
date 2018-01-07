using BadBeeCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Globalization;
using System.Threading;
using log4net;
using BadBee.Core.Models;
using BadBee.Core.DAL;
using BadBee.Core.Providers;

namespace BadBeeCatalog.Controllers
{
    public class DefaultController : Controller
    {
        public static int ITEM_PER_PAGE = 15;
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DefaultController));

        public ActionResult Index(bool? popupClosed)
        {
            try
            {
                if (Request.QueryString["typedPhrase"] != null)
                {
                    BadBeeFilter BadBeeFilter = GlobalVars.BadBeeFilter;
                    BadBeeFilter.PhraseFilter = Request.QueryString["typedPhrase"].ToString();
                    BadBeeFilter.Status = "filter";
                    BadBeeFilter.Page = 1;
                    GlobalVars.BadBeeFilter = BadBeeFilter;
                    return RedirectToAction("Index");
                }

                return View();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            } 
        }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            try
            {
                Response.Cookies.Remove("Language");

                Session["Culture"] = new CultureInfo(lang);
                Response.Cookies.Add(new System.Web.HttpCookie("Language", lang) { Expires = DateTime.Now.AddDays(30) });

                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }            
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            try
            {
                HttpCookie languageCookie = System.Web.HttpContext.Current.Request.Cookies["Language"];
                if (languageCookie != null)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(languageCookie.Value);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCookie.Value);
                }
                base.Initialize(requestContext);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }            
        }

        //public ActionResult DownloadMobileApp(int? type)
        //{
        //    try
        //    {
        //        if (type.HasValue)
        //        {
        //            ViewBag.Platform = type.Value;
        //        }
        //        else
        //        {
        //            ViewBag.Platform = 1;
        //        }

        //        return PartialView();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        throw ex;
        //    }            
        //}

        // POST: /Default/CatalogContent
        public ActionResult CatalogContent(BadBeeFilter filter)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                CatalogContentTableHeadersProvider tableHeadersProvider = new CatalogContentTableHeadersProvider();

                
                ListProvider.ListResult result;
                int pageNumber = (filter.Page ?? 1);
                using (ListProvider provider = new ListProvider())
                {
                    result = provider.GetList(filter, pageNumber, ITEM_PER_PAGE);
                }

                GlobalVars.BadBeeFilter = filter;

                var preRenderObject = new CatalogContent()
                {
                    Items = new StaticPagedList<CvlItem>(result.Items, pageNumber, ITEM_PER_PAGE, result.ItemsCount),
                    DetailsHeaderItems = tableHeadersProvider.CreateDetailsHeaderItems(),
                    TopHeaderItems = tableHeadersProvider.CreateTopHeaderItems()
                };

                int pagesCount = 0;
                if (result.ItemsCount % ITEM_PER_PAGE == 0)
                {
                    pagesCount = result.ItemsCount / ITEM_PER_PAGE;
                }
                else
                {
                    pagesCount = (result.ItemsCount / ITEM_PER_PAGE) + 1;
                }

                filter = GlobalVars.BadBeeFilter;
                filter.TotalCount = pagesCount;
                GlobalVars.BadBeeFilter = filter;

                double timeCount = (DateTime.Now - dtNow).TotalMilliseconds;

                log.Info(string.Format("Execution time with key {0} equal to {1} ms", filter.SearchKey, timeCount));

                if(timeCount > 2500)
                    log.Info(string.Format("Execution time is very long with key {0} equal to {1} ms", filter.SearchKey, timeCount));

                return PartialView(preRenderObject);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }            
        }

        //POST: /Default/GetKeywords
        public ActionResult GetKeywords(string keywordPart)
        {
            using (ListProvider provider = new ListProvider())
            {
                return PartialView(new GetKeywordsModel() { Keywords = provider.GetKeywords(keywordPart) });
            }
        }

        //POST: /Default/GetFilter
        public JsonResult GetFilter()
        {
            try
            {
                return Json(GlobalVars.BadBeeFilter, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public JsonResult CleanFilter()
        {
            try
            { 
            GlobalVars.BadBeeFilter = new BadBeeFilter();
            return Json(GlobalVars.BadBeeFilter, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        
        //POST: /Default/GetYearsListCh
        public JsonResult GetYearsListCh(BadBeeFilter filter)
        {
            try {
            if (filter == null || filter.IsFilterEmpty)
            {
                filter = GlobalVars.BadBeeFilter;
            }
            List<Date> year = new List<Date>();
            using (ListProvider provider = new ListProvider())
            {
                year = provider.GetYearsListCh(filter);
            }
            return Json(new SelectList(year, "DateId", "Date"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetYearsList
        public JsonResult GetYearsList()
        {
            try { 
            List<Date> years = new List<Date>();
            using (ListProvider provider = new ListProvider())
            {
                years = provider.GetYearsList(GlobalVars.BadBeeFilter);
            }
                //List<string> yearsFrom = years.Select(q => q.DateFrom.Value.Year.ToString()).ToList();
                //List<string> yearsTo = years.Select(q => q.DateTo.Value.Year.ToString()).ToList();
                //List<string> yarsList = yearsFrom.Union(yearsTo).ToList();
                var yearsList = years.Select(q => q.Date1).ToList();
            return Json(new SelectList(years, "DateId", "Date1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
       // POST: /Default/GetHeightsListCh
        public JsonResult GetHeightsListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Height> hei = new List<Height>();
                using (ListProvider provider = new ListProvider())
                {
                    hei = provider.GetHeightsListCh(filter);
                }
                return Json(new SelectList(hei, "HeightId", "Height1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetHeightsList
        public JsonResult GetHeightsList()
        {
            try
            {
                List<Height> heights = new List<Height>();
                using (ListProvider provider = new ListProvider())
                {
                    heights = provider.GetHeightsList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(heights, "HeightId", "Height1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetWidthsListCh
        public JsonResult GetWidthsListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Width> wid = new List<Width>();
                using (ListProvider provider = new ListProvider())
                {
                    wid = provider.GetWidthsListCh(filter);
                }
                return Json(new SelectList(wid, "WidthId", "Width1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetWidthsList
        public JsonResult GetWidthsList()
        {
            try
            {
                List<Width> widths = new List<Width>();
                using (ListProvider provider = new ListProvider())
                {
                    widths = provider.GetWidthsList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(widths, "WidthId", "Width1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetThicknessesListCh
        public JsonResult GetThicknessesListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Thickness> thick = new List<Thickness>();
                using (ListProvider provider = new ListProvider())
                {
                    thick = provider.GetThicknessesListCh(filter);
                }
                return Json(new SelectList(thick, "ThicknessId", "Thickness1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }

        //POST: /Default/GetThicknessesList
        public JsonResult GetThicknessesList()
        {
            try
            {
                List<Thickness> thicknesses = new List<Thickness>();
                using (ListProvider provider = new ListProvider())
                {
                    thicknesses = provider.GetThicknessesList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(thicknesses, "ThicknessId", "Thickness1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        //POST: /Default/GetSystemsListCh
        public JsonResult GetSystemsListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Systems> system = new List<Systems>();
                using (ListProvider provider = new ListProvider())
                {
                    system = provider.GetSystemsListCh(filter);
                }
                return Json(new SelectList(system, "SystemId", "Abbreviation"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetSystemsList
        public JsonResult GetSystemsList()
        {
            try
            {
                List<Systems> systems = new List<Systems>();
                using (ListProvider provider = new ListProvider())
                {
                    systems = provider.GetSystemsList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(systems, "SystemId", "Abbreviation"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        //POST: /Default/GetWvaChList
        public JsonResult GetWvaListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Wva> wva = new List<Wva>();
                using (ListProvider provider = new ListProvider())
                {
                    wva = provider.GetWvaChList(filter);
                }
                return Json(new SelectList(wva, "WvaId", "WvaNo"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetWvaList
        public JsonResult GetWvaList()
        {
            try
            {
                List<Wva> wva = new List<Wva>();
                using (ListProvider provider = new ListProvider())
                {
                    wva = provider.GetWvaList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(wva, "WvaId", "WvaNo"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }


        //POST: /Default/GetBadBeeChList
        public JsonResult GetBadBeeListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<BadBee.Core.DAL.BadBee> BadBees = new List<BadBee.Core.DAL.BadBee>();
                using (ListProvider provider = new ListProvider())
                {
                    BadBees = provider.GetBadBeeChList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(BadBees, "BadBeeId", "BadBeeNo"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }

        //POST: /Default/GetBadBeesList
        public JsonResult GetBadBeesList()
        {
            try
            {
                List<BadBee.Core.DAL.BadBee> BadBees = new List<BadBee.Core.DAL.BadBee>();
                using (ListProvider provider = new ListProvider())
                {
                    BadBees = provider.GetBadBeeList(GlobalVars.BadBeeFilter);
                }

                return Json(new SelectList(BadBees, "BadBeeId", "BadBeeNo"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetBrandsListCh
        public JsonResult GetBrandsListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Brand> brands = new List<Brand>();
                using (ListProvider provider = new ListProvider())
                {
                    brands = provider.GetBrandsChList(filter);
                }
                return Json(new SelectList(brands, "BrandId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetBrandsList
        public JsonResult GetBrandsList()
        {
            try
            {
                List<Brand> brands = new List<Brand>();
                using (ListProvider provider = new ListProvider())
                {
                    brands = provider.GetBrandsList(GlobalVars.BadBeeFilter);
                }
                return Json(new SelectList(brands, "BrandId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        //POST: /Default/GetSeriesListCh
        public JsonResult GetSeriesListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Serie> series = new List<Serie>();
                using (ListProvider provider = new ListProvider())
                {
                    series = provider.GetSeriesChList(filter);
                }
                return Json(new SelectList(series, "SerieId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        //// POST: /Default/GetSeriesList
        public JsonResult GetSeriesList()
        {
            try
            {
                List<Serie> series = new List<Serie>();
                using (ListProvider provider = new ListProvider())
                {
                    series = provider.GetSeriesList(GlobalVars.BadBeeFilter);
                }

                return Json(new SelectList(series, "SerieId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        //POST: /Default/GetModelsListCh
        public JsonResult GetModelsListCh(BadBeeFilter filter)
        {
            try
            {
                if (filter == null || filter.IsFilterEmpty)
                {
                    filter = GlobalVars.BadBeeFilter;
                }
                List<Model> models = new List<Model>();
                using (ListProvider provider = new ListProvider())
                {
                    models = provider.GetModelsChList(filter);
                }
                return Json(new SelectList(models, "ModelId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info(filter);
                log.Error(ex);
                throw ex;
            }
        }
        // POST: /Default/GetModelsList
        public JsonResult GetModelsList()
        {
            try
            {
                List<Model> models = new List<Model>();
                using (ListProvider provider = new ListProvider())
                {
                    models = provider.GetModelsList(GlobalVars.BadBeeFilter);
                }

                return Json(new SelectList(models, "ModelId", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        
        [HttpPost]
        public ActionResult Contact(Message message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using(BadBeeEntities entity = new BadBeeEntities())
                    {
                        Message mes = new Message();
                        mes.Content = message.Content;
                        mes.InsertDate = DateTime.Now;
                        mes.IsRead = false;
                        mes.MessageFrom = message.MessageFrom;
                        mes.Name = message.Name;

                        entity.Message.Add(mes);
                        entity.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }

            return RedirectToAction("Index");
        }
    }
}
