using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BadBeeAdminPanel.Models;
using System.Data.Entity;
using System.Web.UI;
using PagedList;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Globalization;
using log4net;
using BadBee.Core.DAL;
using BadBee.Core.Providers;
using BadBee.Core.Models;
using System.Data.OleDb;
using System.Xml;

namespace BadBeeAdminPanel.Controllers
{
    
    [Authorize]
    public class ProductController : Controller
    {   public static int ITEM_PER_PAGE = 50;
        private BadBeeEntities db = new BadBeeEntities();
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ProductController));

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            try
            {
               // HttpCookie languageCookie = System.Web.HttpContext.Current.Request.Cookies["Language"];
               // if (languageCookie != null)
              //  {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("pl-PL");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
              //  }
                base.Initialize(requestContext);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        public ActionResult RefreshData()
        {
            try
            {
            string url = "http://BadBeeCatalog.pl/ReloadEE0B4864";
            return Redirect(url);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        public FileResult DownloadCSVProducts()
        {
            try
            {
                byte[] fileBytes = ExcelProvider.ReadFully_ItemsToCSV("items");
                string fileName = "BadBeeData.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                log.Error(e);
                return null;
            }
        }
        public FileResult DownloadCSVProductsWithIds()
        {
            try
            {
                byte[] fileBytes = ExcelProvider.ReadFully_ItemsToCSV("itemsWithIds");
                string fileName = "BadBeeDataToImport.xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                log.Error(e);
                return null;
            }
        }
       
        public ActionResult ImportProducts()
        {
            return View();
        }

        public ActionResult CreateTable(HttpPostedFileBase file)
        {
            // create the DataSet 
            DataTable dt = new DataTable();
            try
            {
                var path = "";
                if (file != null && file.ContentLength > 0)
                {
                    // extract only the fielname
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    path = System.IO.Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    file.SaveAs(path);
                }

                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();
                var items = db.Item.Include("Model").Include("Model.Serie").Include("Model.Serie.Brand").Include("Model.Year").Include("Model.Year.DateFromFK")
                     .Include("Model.Year.DateToFK").Include("BadBee").Include("BadBee.Wva").Include("BadBee.Systems").Include("BadBee.Dimension")
                     .Include("BadBee.Dimension.Width").Include("BadBee.Dimension.Height").Include("BadBee.Dimension.Thickness").AsQueryable();
              
                
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Item");

                DataTable dataTable = ExcelProvider.ReadAsDataTable(path);


                foreach (DataRow item in dataTable.Rows)
                {
                    ProductModels iitem =  ConvertRowToProductModel(item);
                    AddNewIds(iitem);
                }
                

                GlobalVars.AdvSession = true;
                if (GlobalVars.SearchCaches != null && GlobalVars.SearchCaches.Count >= 10)
                {
                    GlobalVars.SearchCaches = new Dictionary<string, SearchCache>();
                }
                if (GlobalVars.Filters != null && GlobalVars.Filters.Count >= 10)
                {
                    GlobalVars.Filters = new Dictionary<string, BadBeeFilter>();
                }

                GlobalVars.DictionaryCache = new Dictionary<Type, object>();
                ListProvider.FillDictionaryCache();



                return RedirectToAction("Index", new { completed = 1 });


            }
            catch (Exception e)
            {


                SqlBulkCopy bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                bulkCopy.DestinationTableName = "Item";

                bulkCopy.WriteToServer(dt);

                log.Error(e);
                return RedirectToAction("Index", new { completed = 0 });
                //throw e;
            }
        }
        public ProductModels ConvertRowToProductModel(DataRow dr)
        {
            ProductModels pm = new ProductModels();
            pm.BadBeeNumber = dr[0].ToString();
            pm.Fr = dr[1].ToString();
            pm.Brand = dr[2].ToString();
            pm.Height = decimal.Parse(dr[4].ToString());
            pm.ModelName = dr[5].ToString();
            pm.Serie = dr[6].ToString();
            pm.BrakeSystem = dr[7].ToString();
            pm.Thickness = decimal.Parse(dr[8].ToString());
            pm.Width = decimal.Parse(dr[9].ToString());
            pm.Wva = dr[10].ToString();
            pm.WvaDesc = dr[11].ToString();
            
            return pm;
        }

        // GET: Product
        public ActionResult Index(int? page)
        {
            BadBeeFilter BadBeeFilter = GlobalVars.BadBeeFilter;
            GlobalVars.BadBeeFilter = BadBeeFilter;
            return View();
        }

        // POST: /Default/CatalogContent
        public ActionResult CatalogContent(BadBeeFilter filter)
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                GlobalVars.AdvSession = true;
                if (GlobalVars.SearchCaches != null && GlobalVars.SearchCaches.Count >= 10)
                {
                    GlobalVars.SearchCaches = new Dictionary<string, SearchCache>();
                }
                if (GlobalVars.Filters != null && GlobalVars.Filters.Count >= 10)
                {
                    GlobalVars.Filters = new Dictionary<string, BadBeeFilter>();
                }

                CatalogContentTableHeadersProvider tableHeadersProvider = new CatalogContentTableHeadersProvider();


                ListProvider.ListResult result;
                int pageNumber = (filter.Page ?? 1);
                using (ListProvider provider = new ListProvider())
                {
                    result = provider.GetList(filter, pageNumber, ITEM_PER_PAGE);
                }

                GlobalVars.BadBeeFilter = filter;

                if (GlobalVars.BadBeeFilter.BadBeeNumbersList == null)
                    GlobalVars.BadBeeFilter.BadBeeNumbersList = new List<int>();
                if (GlobalVars.BadBeeFilter.BrandsList == null)
                    GlobalVars.BadBeeFilter.BrandsList = new List<int>();
                if (GlobalVars.BadBeeFilter.DateYearsList == null)
                    GlobalVars.BadBeeFilter.DateYearsList = new List<int>();
                if (GlobalVars.BadBeeFilter.HeightsList == null)
                    GlobalVars.BadBeeFilter.HeightsList = new List<int>();
                if (GlobalVars.BadBeeFilter.ModelsList == null)
                    GlobalVars.BadBeeFilter.ModelsList = new List<int>();
                if (GlobalVars.BadBeeFilter.SeriesList == null)
                    GlobalVars.BadBeeFilter.SeriesList = new List<int>();
                if (GlobalVars.BadBeeFilter.SystemsList == null)
                    GlobalVars.BadBeeFilter.SystemsList = new List<int>();
                if (GlobalVars.BadBeeFilter.ThicknessesList == null)
                    GlobalVars.BadBeeFilter.ThicknessesList = new List<int>();
                if (GlobalVars.BadBeeFilter.WidthsList == null)
                    GlobalVars.BadBeeFilter.WidthsList = new List<int>();
                if (GlobalVars.BadBeeFilter.WvasList == null)
                    GlobalVars.BadBeeFilter.WvasList = new List<int>();

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

                if (timeCount > 2500)
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

      

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Item item = db.Item.Where(q=>q.Id==id).FirstOrDefault();
                if (item == null)
                {
                    return HttpNotFound();
                }
                return View(item);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        // POST: Crosses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

            Item item = db.Item.Where(q=>q.Id==id).FirstOrDefault();
            db.Item.Remove(item);
           db.SaveChanges();
 
            GlobalVars.DictionaryCache = new Dictionary<Type, object>();
            ListProvider.FillDictionaryCache();

            return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
       
        public ActionResult Edit(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                var product = db.Item.Where(q => q.Id == id).FirstOrDefault();
                ProductModels productModels = ConvertToProductModel(product);
                if (productModels == null)
            {
                return HttpNotFound();
            }
            return View(productModels);
                
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        public ActionResult Details(int? id)
        {
            try
            {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item product = db.Item.Where(q=>q.Id==id).FirstOrDefault();
            ProductModels productModels = ConvertToProductModel(product);
            if (productModels == null)
            {
                return HttpNotFound();
            }
            return View(productModels);

            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        private ProductModels ConvertToProductModel(Item product)
        {
            ProductModels productModels = new ProductModels();
            productModels.BadBeeNumber = product.BadBee.BadBeeNo;
            productModels.BadBeeNumberId = product.BadBee.BadBeeId;
            productModels.BrakeSystem = product.BadBee.Systems.Abbreviation;
            productModels.Brand = product.Model.Serie.Brand.Name;
            productModels.BrandId = product.Model.Serie.Brand.BrandId;
            productModels.DateFrom = product.Model.Year.DateFromFK.Date1 == string.Empty ? 0 : int.Parse(product.Model.Year.DateFromFK.Date1);
            productModels.DateTo = product.Model.Year.DateToFK.Date1==string.Empty ? 0 : int.Parse(product.Model.Year.DateToFK.Date1);
            productModels.Fr = product.BadBee.FR;
            productModels.Height = product.BadBee.Dimension.Height.Height1 ?? default(decimal);
            productModels.HeightId = product.BadBee.Dimension.Height.HeightId;
            productModels.Id = product.Id;
            productModels.ModelId = product.Model.ModelId;
            productModels.ModelName = product.Model.Name;
            productModels.Serie = product.Model.Serie.Name;
            productModels.SerieId = product.Model.Serie.SerieId;
            productModels.SystemId = product.BadBee.Systems.SystemId;
            productModels.Thickness = product.BadBee.Dimension.Thickness.Thickness1??default(decimal);
            productModels.ThicknessId = product.BadBee.Dimension.Thickness.ThicknessId;
            productModels.Width = product.BadBee.Dimension.Width.Width1??default(decimal);
            productModels.WidthId = product.BadBee.Dimension.Width.WidthId;
            productModels.Wva = product.BadBee.Wva.WvaNo;
            productModels.WvaDesc = product.BadBee.Wva.Description;
            productModels.WvaId = product.BadBee.Wva.WvaId;
            
            return productModels;
        }

        public ActionResult Create()
        {
            return View(new ProductModels());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModels model, string returnUrl = null)
        {
            try
            {
            if (ModelState.IsValid)
            {
                string logoPath = "";
                string logoRelativePath = "";
               
                var modified = db.Item.Where(s => s.Id == model.Id).FirstOrDefault();
                    var original = db.Item.Where(s => s.Id == model.Id).FirstOrDefault();

                    //brand - wymagany

                    if (!string.IsNullOrEmpty(model.Brand))
                    {
                        if (db.Brand.Any(row => row.Name == model.Brand))
                        {
                            // Brand br = db.Brand.Where(q => q.Name == modified.Model.Serie.Brand.Name).SingleOrDefault();
                            //jest w bazie 
                            // modified.Model.Serie.Brand.BrandId = br.BrandId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newBrand = new Brand();
                            var nb = db.Brand.OrderByDescending(q => q.BrandId).Select(q => q).FirstOrDefault();
                            int newid = nb.BrandId + 1;
                            newBrand.Name = model.Brand;
                            newBrand.BrandId = newid;
                            //  modified.Model.Serie.Brand.BrandId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Brand.Add(newBrand);
                                dbCtx.SaveChanges();
                            }
                        }
                    }

                    //seria - niewymagana
                    //       modified.Model.Serie.Name = model.Serie;

                    if (model.Serie == null)
                    {
                        //      modified.Model.Serie.Name = "";
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                        if (db.Serie.Any(row => row.BrandId == br.BrandId && row.Name == ""))
                        {
                            //seria pusta, brand pustą serią jest już w bazie
                            //   Serie ser = db.Serie.Where(q => q.BrandId == modified.Model.Serie.BrandId).SingleOrDefault();
                            //   modified.Model.Serie.SerieId = ser.SerieId;
                        }
                        else
                        {
                            //seria pusta, brandu z pustą serią nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = "";
                            newproduct.SerieId = newid;
                            newproduct.BrandId = br.BrandId;
                            //     modified.Model.Serie.SerieId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Serie.Add(newproduct);
                                dbCtx.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                        if (db.Serie.Any(row => row.Name == model.Serie))
                        {
                            //jest w bazie 
                            //  Serie ser = db.Serie.Where(q => q.Name == modified.Model.Serie.Name).SingleOrDefault();
                            //   modified.Model.Serie.SerieId = ser.SerieId;

                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = model.Serie;
                            newproduct.SerieId = newid;
                            newproduct.BrandId = br.BrandId;
                            //       modified.Model.Serie.SerieId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Serie.Add(newproduct);
                                dbCtx.SaveChanges();
                            }
                        }
                    }

                    //model - wymagany
                    //     modified.Model.Name = model.ModelName;

                    if (db.Model.Any(row => row.Name == model.ModelName))
                    {
                        Model mod = db.Model.Where(q => q.Name == model.ModelName).SingleOrDefault();
                        //jest w bazie 
                        modified.ModelId = mod.ModelId;
                    }
                    else
                    {
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();
                        Serie serie = db.Serie.Where(q => q.Name == (model.Serie == null ? string.Empty : model.Serie)).Where(q => q.BrandId == br.BrandId).SingleOrDefault();

                        //nie ma w bazie
                        var newproduct = new Model();
                        var np = db.Model.OrderByDescending(q => q.ModelId).Select(q => q).FirstOrDefault();
                        int newid = np.ModelId + 1;
                        newproduct.Name = model.ModelName;
                        newproduct.ModelId = newid;
                        newproduct.SerieId = serie.SerieId;

                        if (model.DateFrom == 0 && model.DateTo == 0)
                        {
                            newproduct.YearId = 0;
                        }
                        else
                        {
                            Year year = db.Year.Where(q => q.DateFromFK.Date1 == model.DateFrom.ToString()).Where(q => q.DateToFK.Date1 == model.DateTo.ToString()).SingleOrDefault();
                            if (year != null)
                            {
                                newproduct.YearId = year.YearId;
                            }
                            else //dodaje nową datę
                            {
                                var newYear = new Year();
                                Date yf = db.Date.Where(q => q.Date1 == model.DateFrom.ToString()).SingleOrDefault();
                                Date yt = db.Date.Where(q => q.Date1 == model.DateTo.ToString()).SingleOrDefault();

                                if (yf != null)
                                {
                                    newYear.DateFromId = yf.DateId;
                                }
                                else
                                {
                                    var date = new Date();
                                    var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                                    int newidDate = nd.DateId + 1;
                                    date.DateId = newidDate;
                                    date.Date1 = model.DateFrom.ToString();
                                }
                                if (yt != null)
                                {
                                    newYear.DateToId = yt.DateId;
                                }
                                else
                                {
                                    var date = new Date();
                                    var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                                    int newidDate = nd.DateId + 1;
                                    date.DateId = newidDate;
                                    date.Date1 = model.DateTo.ToString();
                                }

                            }


                        }

                        modified.ModelId = newid;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Model.Add(newproduct);
                            dbCtx.SaveChanges();
                        }
                    }
                    //height
                    Height he = db.Height.Where(q => q.Height1 == model.Height).SingleOrDefault();
                    if (he == null)
                    {
                        he = new Height();
                        var nh = db.Height.OrderByDescending(q => q.HeightId).Select(q => q).FirstOrDefault();
                        int newidHeight = nh.HeightId + 1;
                        he.Height1 = model.Height;
                        he.HeightId = newidHeight;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Height.Add(he);
                            dbCtx.SaveChanges();
                        }
                    }

                    //width
                    Width wi = db.Width.Where(q => q.Width1 == model.Width).SingleOrDefault();
                    if (he == null)
                    {
                        wi = new Width();
                        var nw = db.Width.OrderByDescending(q => q.WidthId).Select(q => q).FirstOrDefault();
                        int newidWidth = nw.WidthId + 1;
                        wi.Width1 = model.Width;
                        wi.WidthId = newidWidth;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Width.Add(wi);
                            dbCtx.SaveChanges();
                        }
                    }

                    //thickness
                    Thickness th = db.Thickness.Where(q => q.Thickness1 == model.Thickness).SingleOrDefault();
                    if (th == null)
                    {
                        th = new Thickness();
                        var nw = db.Thickness.OrderByDescending(q => q.ThicknessId).Select(q => q).FirstOrDefault();
                        int newidThickness = nw.ThicknessId + 1;
                        th.Thickness1 = model.Thickness;
                        th.ThicknessId = newidThickness;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Thickness.Add(th);
                            dbCtx.SaveChanges();
                        }
                    }

                    //dimension

                    Dimension dim = db.Dimension.Where(q => q.HeightId == he.HeightId).Where(q => q.ThicknessId == th.ThicknessId).Where(q => q.WidthId == wi.WidthId).SingleOrDefault();
                    if (dim == null)
                    {
                        dim = new Dimension();
                        var nd = db.Dimension.OrderByDescending(q => q.DimensionId).Select(q => q).FirstOrDefault();
                        int newidDim = nd.DimensionId + 1;
                        dim.DimensionId = newidDim;
                        dim.WidthId = wi.WidthId;
                        dim.HeightId = he.HeightId;
                        dim.ThicknessId = th.ThicknessId;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Dimension.Add(dim);
                            dbCtx.SaveChanges();
                        }
                    }

                    //systems
                    Systems sys = db.Systems.Where(q => q.Abbreviation == model.BrakeSystem).SingleOrDefault();
                    if (sys == null)
                    {
                        sys = new Systems();
                        var ns = db.Systems.OrderByDescending(q => q.SystemId).Select(q => q).FirstOrDefault();
                        int newidSys = ns.SystemId + 1;
                        sys.SystemId = newidSys;
                        sys.Abbreviation = model.BrakeSystem;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Systems.Add(sys);
                            dbCtx.SaveChanges();
                        }
                    }

                    Wva wva = db.Wva.Where(q => q.WvaNo == model.Wva).Where(q => q.Description == model.WvaDesc).SingleOrDefault();
                    if (wva == null)
                    {
                        Wva wvaupdate = db.Wva.Where(q => q.WvaNo == model.Wva).SingleOrDefault();
                        if (wvaupdate != null)
                        {
                            wvaupdate.Description = model.WvaDesc;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                db.Entry(wvaupdate).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            wva = new Wva();
                            var nw = db.Wva.OrderByDescending(q => q.WvaId).Select(q => q).FirstOrDefault();
                            int newidwva = nw.WvaId + 1;
                            wva.WvaId = newidwva;
                            wva.WvaNo = model.Wva;
                            wva.Description = model.WvaDesc;

                        using (var dbCtx = new BadBeeEntities())
                        {
                                dbCtx.Wva.Add(wva);
                            dbCtx.SaveChanges();
                        }
                        }
                        
                    }



                    BadBee.Core.DAL.BadBee bb = db.BadBee.Where(q => q.BadBeeNo == model.BadBeeNumber).Where(q => q.FR == model.Fr).Where(q => q.DimensionId == dim.DimensionId).Where(q => q.SystemId == sys.SystemId).Where(q => q.WvaId == wva.WvaId).SingleOrDefault();
                    if (bb == null)
                    {
                        bb = new BadBee.Core.DAL.BadBee();
                        var np = db.BadBee.OrderByDescending(q => q.BadBeeId).Select(q => q).FirstOrDefault();
                        int newid = np.BadBeeId + 1;
                        bb.BadBeeId = newid;
                        bb.BadBeeNo = model.BadBeeNumber;
                        bb.DimensionId = dim.DimensionId;
                        bb.FR = model.Fr;
                        bb.SystemId = sys.SystemId;
                        bb.WvaId = wva.WvaId;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.BadBee.Add(bb);
                            dbCtx.SaveChanges();
                        }
                    }
                    int originalBadBeeId = original.BadBeeId;

                    modified.BadBeeId = bb.BadBeeId;
                    if (original.ModelId!=modified.ModelId|| originalBadBeeId!=modified.BadBeeId)
                    {
                        using (var dbCtx = new BadBeeEntities())
                        {
                            originalBadBeeId = 53;
                            Item orig = db.Item.Where(q => q.BadBeeId == originalBadBeeId).SingleOrDefault();

                            int aa = orig.BadBeeId;
                        
                            db.Item.Remove(orig);
                            db.Item.Add(modified);
                            db.SaveChanges();
                        }
                    }
                    else
                    {

                    }
                    

                   

            //        db.Entry(modified).State = EntityState.Modified;
            //        db.SaveChanges();
                        

               // }
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                    GlobalVars.DictionaryCache = new Dictionary<Type, object>();
                    ListProvider.FillDictionaryCache();

                    return RedirectToAction("Index");
            }
                GlobalVars.DictionaryCache = new Dictionary<Type, object>();
                ListProvider.FillDictionaryCache();

                return View(model);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Item newProd = new Item();

                    var newProductId = db.Item.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    newProd.Id = newProductId.Id + 1;
                    

                    //brand - wymagany

                    if (!string.IsNullOrEmpty(model.Brand))
                    {
                        if (db.Brand.Any(row => row.Name == model.Brand))
                        {
                            // Brand br = db.Brand.Where(q => q.Name == newProd.Model.Serie.Brand.Name).SingleOrDefault();
                            //jest w bazie 
                            // newProd.Model.Serie.Brand.BrandId = br.BrandId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newBrand = new Brand();
                            var nb = db.Brand.OrderByDescending(q => q.BrandId).Select(q => q).FirstOrDefault();
                            int newid = nb.BrandId + 1;
                            newBrand.Name = model.Brand;
                            newBrand.BrandId = newid;
                          //  newProd.Model.Serie.Brand.BrandId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Brand.Add(newBrand);
                                dbCtx.SaveChanges();
                            }
                        }
                    }

                    //seria - niewymagana
             //       newProd.Model.Serie.Name = model.Serie;

                    if (model.Serie == null)
                    {
                        //      newProd.Model.Serie.Name = "";
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                        if (db.Serie.Any(row => row.BrandId ==br.BrandId && row.Name == ""))
                        {
                            //seria pusta, brand pustą serią jest już w bazie
                            //   Serie ser = db.Serie.Where(q => q.BrandId == newProd.Model.Serie.BrandId).SingleOrDefault();
                            //   newProd.Model.Serie.SerieId = ser.SerieId;
                        }
                        else
                        {
                            //seria pusta, brandu z pustą serią nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = "";
                            newproduct.SerieId = newid;
                            newproduct.BrandId =br.BrandId;
                       //     newProd.Model.Serie.SerieId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Serie.Add(newproduct);
                                dbCtx.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                        if (db.Serie.Any(row => row.Name == model.Serie))
                        {
                            //jest w bazie 
                            //  Serie ser = db.Serie.Where(q => q.Name == newProd.Model.Serie.Name).SingleOrDefault();
                            //   newProd.Model.Serie.SerieId = ser.SerieId;

                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = model.Serie;
                            newproduct.SerieId = newid;
                            newproduct.BrandId = br.BrandId;
                     //       newProd.Model.Serie.SerieId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Serie.Add(newproduct);
                                dbCtx.SaveChanges();
                            }
                        }
                    }

                    //model - wymagany
               //     newProd.Model.Name = model.ModelName;

                    if (db.Model.Any(row => row.Name == model.ModelName))
                    {
                        Model mod = db.Model.Where(q => q.Name == model.ModelName).SingleOrDefault();
                        //jest w bazie 
                        newProd.ModelId = mod.ModelId;
                    }
                    else
                    {
                        Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();
                        Serie serie = db.Serie.Where(q => q.Name == (model.Serie==null?string.Empty:model.Serie)).Where(q=>q.BrandId==br.BrandId).SingleOrDefault();

                        //nie ma w bazie
                        var newproduct = new Model();
                        var np = db.Model.OrderByDescending(q => q.ModelId).Select(q => q).FirstOrDefault();
                        int newid = np.ModelId + 1;
                        newproduct.Name = model.ModelName;
                        newproduct.ModelId = newid;
                        newproduct.SerieId = serie.SerieId;

                        if (model.DateFrom==0 && model.DateTo==0)
                        {
                            newproduct.YearId = 0;
                        }
                        else
                        {
                            Year year = db.Year.Where(q => q.DateFromFK.Date1 == model.DateFrom.ToString()).Where(q => q.DateToFK.Date1 == model.DateTo.ToString()).SingleOrDefault();
                            if (year!=null)
                            {
                                newproduct.YearId = year.YearId;
                            }
                            else //dodaje nową datę
                            {
                                var newYear = new Year();
                                Date yf = db.Date.Where(q => q.Date1 == model.DateFrom.ToString()).SingleOrDefault();
                                Date yt = db.Date.Where(q => q.Date1 == model.DateTo.ToString()).SingleOrDefault();

                                if (yf!=null)
                                {
                                    newYear.DateFromId = yf.DateId;
                                }
                                else
                                {
                                    var date = new Date();
                                    var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                                    int newidDate = nd.DateId + 1;
                                    date.DateId = newidDate;
                                    date.Date1 = model.DateFrom.ToString();
                                }
                                if (yt != null)
                                {
                                    newYear.DateToId = yt.DateId;
                                }
                                else
                                {
                                    var date = new Date();
                                    var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                                    int newidDate = nd.DateId + 1;
                                    date.DateId = newidDate;
                                    date.Date1 = model.DateTo.ToString();
                                }

                            }
                            

                        }

                        newProd.ModelId = newid;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Model.Add(newproduct);
                            dbCtx.SaveChanges();
                        }
                    }
                    //height
                    Height he = db.Height.Where(q => q.Height1 == model.Height).SingleOrDefault();
                    if (he==null)
                    {
                        he = new Height();
                        var nh = db.Height.OrderByDescending(q => q.HeightId).Select(q => q).FirstOrDefault();
                        int newidHeight = nh.HeightId + 1;
                        he.Height1 = model.Height;
                        he.HeightId = newidHeight;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Height.Add(he);
                            dbCtx.SaveChanges();
                        }
                    }

                    //width
                    Width wi = db.Width.Where(q => q.Width1 == model.Width).SingleOrDefault();
                    if (he == null)
                    {
                        wi = new Width();
                        var nw = db.Width.OrderByDescending(q => q.WidthId).Select(q => q).FirstOrDefault();
                        int newidWidth = nw.WidthId + 1;
                        wi.Width1 = model.Width;
                        wi.WidthId = newidWidth;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Width.Add(wi);
                            dbCtx.SaveChanges();
                        }
                    }

                    //thickness
                    Thickness th = db.Thickness.Where(q => q.Thickness1 == model.Thickness).SingleOrDefault();
                    if (th == null)
                    {
                        th = new Thickness();
                        var nw = db.Thickness.OrderByDescending(q => q.ThicknessId).Select(q => q).FirstOrDefault();
                        int newidThickness = nw.ThicknessId + 1;
                        th.Thickness1 = model.Thickness;
                        th.ThicknessId = newidThickness;
                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Thickness.Add(th);
                            dbCtx.SaveChanges();
                        }
                    }

                    //dimension

                    Dimension dim = db.Dimension.Where(q => q.HeightId == he.HeightId).Where(q => q.ThicknessId == th.ThicknessId).Where(q => q.WidthId == wi.WidthId).SingleOrDefault();
                    if (dim==null)
                    {
                        dim = new Dimension();
                        var nd = db.Dimension.OrderByDescending(q => q.DimensionId).Select(q => q).FirstOrDefault();
                        int newidDim = nd.DimensionId + 1;
                        dim.DimensionId = newidDim;
                        dim.WidthId = wi.WidthId;
                        dim.HeightId = he.HeightId;
                        dim.ThicknessId = th.ThicknessId;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Dimension.Add(dim);
                            dbCtx.SaveChanges();
                        }
                    }

                    //systems
                    Systems sys = db.Systems.Where(q => q.Abbreviation == model.BrakeSystem).SingleOrDefault();
                    if (sys == null)
                    {
                        sys = new Systems();
                        var ns = db.Systems.OrderByDescending(q => q.SystemId).Select(q => q).FirstOrDefault();
                        int newidSys = ns.SystemId + 1;
                        sys.SystemId = newidSys;
                        sys.Abbreviation = model.BrakeSystem;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.Systems.Add(sys);
                            dbCtx.SaveChanges();
                        }
                    }

                    Wva wva = db.Wva.Where(q => q.WvaNo == model.Wva).Where(q => q.Description == model.WvaDesc).SingleOrDefault();
                    if (wva == null)
                    {
                        Wva wvaupdate = db.Wva.Where(q => q.WvaNo == model.Wva).SingleOrDefault();
                        if (wvaupdate!=null)
                        {
                            wvaupdate.Description = model.WvaDesc;
                        }
                        else
                        {
                            wva = new Wva();
                            var nw = db.Wva.OrderByDescending(q => q.WvaId).Select(q => q).FirstOrDefault();
                            int newidwva = nw.WvaId + 1;
                            wva.WvaId = newidwva;
                            wva.WvaNo=model.Wva;
                            wva.Description = model.WvaDesc; 
                        }
                        using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Wva.Add(wva);
                                dbCtx.SaveChanges();
                            }
                    }



                    BadBee.Core.DAL.BadBee bb = db.BadBee.Where(q => q.BadBeeNo == model.BadBeeNumber).Where(q => q.FR == model.Fr).SingleOrDefault();
                    if (bb==null)
                    {
                        bb = new BadBee.Core.DAL.BadBee();
                        var np = db.BadBee.OrderByDescending(q => q.BadBeeId).Select(q => q).FirstOrDefault();
                        int newid = np.BadBeeId + 1;
                        bb.BadBeeId = newid;
                        bb.BadBeeNo = model.BadBeeNumber;
                        bb.DimensionId = dim.DimensionId;
                        bb.FR = model.Fr;
                        bb.SystemId = sys.SystemId;
                        bb.WvaId = wva.WvaId;

                        using (var dbCtx = new BadBeeEntities())
                        {
                            dbCtx.BadBee.Add(bb);
                            dbCtx.SaveChanges();
                        }
                    }

                    newProd.BadBeeId = bb.BadBeeId;

                    using (var dbCtx = new BadBeeEntities())
                    {
                        db.Item.Add(newProd);
                        db.SaveChanges();
                    }
                        
                    
                   

                }
                GlobalVars.DictionaryCache = new Dictionary<Type, object>();
               ListProvider.FillDictionaryCache();

                return RedirectToAction("Index");


            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        //POST: /Product/GetBrandsList
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

        // POST: /Product/GetSeriesList
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

        //POST: /Product/GetModelsList
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

        //POST: /Default/GetYearsList
        public JsonResult GetYearsList()
        {
            try
            {
                List<Date> years = new List<Date>();
                using (ListProvider provider = new ListProvider())
                {
                    years = provider.GetYearsList(GlobalVars.BadBeeFilter);
                }

                return Json(new SelectList(years, "DateId", "Date1"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
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

        //POST: /Product/GetWidthsList
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
        //POST: /Product/GetHeightsList
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
        //POST: /Product/GetThicknessesList
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
        //POST: /Product/GetSystemsList
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
        
        //POST: /Product/GetBadBeesList
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

        public ActionResult AddNewIds(ProductModels model)
        {
            Item newProd = new Item();

            var newProductId = db.Item.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            newProd.Id = newProductId.Id + 1;


            //brand - wymagany

            if (!string.IsNullOrEmpty(model.Brand))
            {
                if (db.Brand.Any(row => row.Name == model.Brand))
                {
                    // Brand br = db.Brand.Where(q => q.Name == newProd.Model.Serie.Brand.Name).SingleOrDefault();
                    //jest w bazie 
                    // newProd.Model.Serie.Brand.BrandId = br.BrandId;
                }
                else
                {
                    //nie ma w bazie
                    var newBrand = new Brand();
                    var nb = db.Brand.OrderByDescending(q => q.BrandId).Select(q => q).FirstOrDefault();
                    int newid = nb.BrandId + 1;
                    newBrand.Name = model.Brand;
                    newBrand.BrandId = newid;
                    //  newProd.Model.Serie.Brand.BrandId = newid;
                    using (var dbCtx = new BadBeeEntities())
                    {
                        dbCtx.Brand.Add(newBrand);
                        dbCtx.SaveChanges();
                    }
                }
            }

            //seria - niewymagana
            //       newProd.Model.Serie.Name = model.Serie;

            if (model.Serie == null)
            {
                //      newProd.Model.Serie.Name = "";
                Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                if (db.Serie.Any(row => row.BrandId == br.BrandId && row.Name == ""))
                {
                    //seria pusta, brand pustą serią jest już w bazie
                    //   Serie ser = db.Serie.Where(q => q.BrandId == newProd.Model.Serie.BrandId).SingleOrDefault();
                    //   newProd.Model.Serie.SerieId = ser.SerieId;
                }
                else
                {
                    //seria pusta, brandu z pustą serią nie ma w bazie
                    var newproduct = new Serie();
                    var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                    int newid = np.SerieId + 1;
                    newproduct.Name = "";
                    newproduct.SerieId = newid;
                    newproduct.BrandId = br.BrandId;
                    //     newProd.Model.Serie.SerieId = newid;
                    using (var dbCtx = new BadBeeEntities())
                    {
                        dbCtx.Serie.Add(newproduct);
                        dbCtx.SaveChanges();
                    }
                }
            }
            else
            {
                Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();

                if (db.Serie.Any(row => row.Name == model.Serie))
                {
                    //jest w bazie 
                    //  Serie ser = db.Serie.Where(q => q.Name == newProd.Model.Serie.Name).SingleOrDefault();
                    //   newProd.Model.Serie.SerieId = ser.SerieId;

                }
                else
                {
                    //nie ma w bazie
                    var newproduct = new Serie();
                    var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                    int newid = np.SerieId + 1;
                    newproduct.Name = model.Serie;
                    newproduct.SerieId = newid;
                    newproduct.BrandId = br.BrandId;
                    //       newProd.Model.Serie.SerieId = newid;
                    using (var dbCtx = new BadBeeEntities())
                    {
                        dbCtx.Serie.Add(newproduct);
                        dbCtx.SaveChanges();
                    }
                }
            }

            //model - wymagany
            //     newProd.Model.Name = model.ModelName;

            if (db.Model.Any(row => row.Name == model.ModelName))
            {
                Model mod = db.Model.Where(q => q.Name == model.ModelName).SingleOrDefault();
                //jest w bazie 
                newProd.ModelId = mod.ModelId;
            }
            else
            {
                Brand br = db.Brand.Where(q => q.Name == model.Brand).SingleOrDefault();
                Serie serie = db.Serie.Where(q => q.Name == (model.Serie == null ? string.Empty : model.Serie)).Where(q => q.BrandId == br.BrandId).SingleOrDefault();

                //nie ma w bazie
                var newproduct = new Model();
                var np = db.Model.OrderByDescending(q => q.ModelId).Select(q => q).FirstOrDefault();
                int newid = np.ModelId + 1;
                newproduct.Name = model.ModelName;
                newproduct.ModelId = newid;
                newproduct.SerieId = serie.SerieId;

                if (model.DateFrom == 0 && model.DateTo == 0)
                {
                    newproduct.YearId = 0;
                }
                else
                {
                    Year year = db.Year.Where(q => q.DateFromFK.Date1 == model.DateFrom.ToString()).Where(q => q.DateToFK.Date1 == model.DateTo.ToString()).SingleOrDefault();
                    if (year != null)
                    {
                        newproduct.YearId = year.YearId;
                    }
                    else //dodaje nową datę
                    {
                        var newYear = new Year();
                        Date yf = db.Date.Where(q => q.Date1 == model.DateFrom.ToString()).SingleOrDefault();
                        Date yt = db.Date.Where(q => q.Date1 == model.DateTo.ToString()).SingleOrDefault();

                        if (yf != null)
                        {
                            newYear.DateFromId = yf.DateId;
                        }
                        else
                        {
                            var date = new Date();
                            var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                            int newidDate = nd.DateId + 1;
                            date.DateId = newidDate;
                            date.Date1 = model.DateFrom.ToString();
                        }
                        if (yt != null)
                        {
                            newYear.DateToId = yt.DateId;
                        }
                        else
                        {
                            var date = new Date();
                            var nd = db.Date.OrderByDescending(q => q.DateId).Select(q => q).FirstOrDefault();
                            int newidDate = nd.DateId + 1;
                            date.DateId = newidDate;
                            date.Date1 = model.DateTo.ToString();
                        }

                    }


                }

                newProd.ModelId = newid;

                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Model.Add(newproduct);
                    dbCtx.SaveChanges();
                }
            }
            //height
            Height he = db.Height.Where(q => q.Height1 == model.Height).SingleOrDefault();
            if (he == null)
            {
                he = new Height();
                var nh = db.Height.OrderByDescending(q => q.HeightId).Select(q => q).FirstOrDefault();
                int newidHeight = nh.HeightId + 1;
                he.Height1 = model.Height;
                he.HeightId = newidHeight;
                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Height.Add(he);
                    dbCtx.SaveChanges();
                }
            }

            //width
            Width wi = db.Width.Where(q => q.Width1 == model.Width).SingleOrDefault();
            if (he == null)
            {
                wi = new Width();
                var nw = db.Width.OrderByDescending(q => q.WidthId).Select(q => q).FirstOrDefault();
                int newidWidth = nw.WidthId + 1;
                wi.Width1 = model.Width;
                wi.WidthId = newidWidth;
                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Width.Add(wi);
                    dbCtx.SaveChanges();
                }
            }

            //thickness
            Thickness th = db.Thickness.Where(q => q.Thickness1 == model.Thickness).SingleOrDefault();
            if (th == null)
            {
                th = new Thickness();
                var nw = db.Thickness.OrderByDescending(q => q.ThicknessId).Select(q => q).FirstOrDefault();
                int newidThickness = nw.ThicknessId + 1;
                th.Thickness1 = model.Thickness;
                th.ThicknessId = newidThickness;
                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Thickness.Add(th);
                    dbCtx.SaveChanges();
                }
            }

            //dimension

            Dimension dim = db.Dimension.Where(q => q.HeightId == he.HeightId).Where(q => q.ThicknessId == th.ThicknessId).Where(q => q.WidthId == wi.WidthId).SingleOrDefault();
            if (dim == null)
            {
                dim = new Dimension();
                var nd = db.Dimension.OrderByDescending(q => q.DimensionId).Select(q => q).FirstOrDefault();
                int newidDim = nd.DimensionId + 1;
                dim.DimensionId = newidDim;
                dim.WidthId = wi.WidthId;
                dim.HeightId = he.HeightId;
                dim.ThicknessId = th.ThicknessId;

                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Dimension.Add(dim);
                    dbCtx.SaveChanges();
                }
            }

            //systems
            Systems sys = db.Systems.Where(q => q.Abbreviation == model.BrakeSystem).SingleOrDefault();
            if (sys == null)
            {
                sys = new Systems();
                var ns = db.Systems.OrderByDescending(q => q.SystemId).Select(q => q).FirstOrDefault();
                int newidSys = ns.SystemId + 1;
                sys.SystemId = newidSys;
                sys.Abbreviation = model.BrakeSystem;

                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Systems.Add(sys);
                    dbCtx.SaveChanges();
                }
            }

            Wva wva = db.Wva.Where(q => q.WvaNo == model.Wva).Where(q => q.Description == model.WvaDesc).SingleOrDefault();
            if (wva == null)
            {
                Wva wvaupdate = db.Wva.Where(q => q.WvaNo == model.Wva).SingleOrDefault();
                if (wvaupdate != null)
                {
                    wvaupdate.Description = model.WvaDesc;
                }
                else
                {
                    wva = new Wva();
                    var nw = db.Wva.OrderByDescending(q => q.WvaId).Select(q => q).FirstOrDefault();
                    int newidwva = nw.WvaId + 1;
                    wva.WvaId = newidwva;
                    wva.WvaNo = model.Wva;
                    wva.Description = model.WvaDesc;
                }
                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.Wva.Add(wva);
                    dbCtx.SaveChanges();
                }
            }



            BadBee.Core.DAL.BadBee bb = db.BadBee.Where(q => q.BadBeeNo == model.BadBeeNumber).Where(q => q.FR == model.Fr).SingleOrDefault();
            if (bb == null)
            {
                bb = new BadBee.Core.DAL.BadBee();
                var np = db.BadBee.OrderByDescending(q => q.BadBeeId).Select(q => q).FirstOrDefault();
                int newid = np.BadBeeId + 1;
                bb.BadBeeId = newid;
                bb.BadBeeNo = model.BadBeeNumber;
                bb.DimensionId = dim.DimensionId;
                bb.FR = model.Fr;
                bb.SystemId = sys.SystemId;
                bb.WvaId = wva.WvaId;

                using (var dbCtx = new BadBeeEntities())
                {
                    dbCtx.BadBee.Add(bb);
                    dbCtx.SaveChanges();
                }
            }

            newProd.BadBeeId = bb.BadBeeId;

            using (var dbCtx = new BadBeeEntities())
            {
                db.Item.Add(newProd);
                db.SaveChanges();
            }


            return RedirectToAction("Index");
            
          
        }
        public ActionResult GeneratePDF()
        {
            PDFProvider pdfProvider = new PDFProvider();
            pdfProvider.GeneratePDFCatalog();

            return RedirectToAction("Index");
        }
    }
}