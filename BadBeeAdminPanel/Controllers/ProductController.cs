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
                string query = "SELECT * FROM [ItemsDb]";
                SqlCommand cmd = new SqlCommand(query, conn);


                dt.Load(cmd.ExecuteReader());



                db.Database.ExecuteSqlCommand("TRUNCATE TABLE ItemsDb");
                //throw new NotImplementedException();

                DataTable dataTable = ExcelProvider.ReadAsDataTable(path);

                SqlBulkCopy bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

                bulkCopy.DestinationTableName = "ItemsDb";

                bulkCopy.WriteToServer(dataTable);

                AddNewIds();

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

                bulkCopy.DestinationTableName = "ItemsDb";

                bulkCopy.WriteToServer(dt);

                log.Error(e);
                return RedirectToAction("Index", new { completed = 0 });
                //throw e;
            }
        }
      

        // GET: Product
        public ActionResult Index(int? page)
        {
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

                    modified.BadBeeId = bb.BadBeeId;

                    
                    db.Entry(modified).State = EntityState.Modified;
                    db.SaveChanges();
                        

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

        public ActionResult AddNewIds()
        {
            //try
            //{
            //List<ItemsDb> newItems = db.ItemsDb.Where(q => 
            //    (q.BrandId == null && q.DrumDiameterId == null && q.HeightId == null && q.BadBeeNumberId == null && q.ModelId == null && 
            //    q.PictureId == null && q.RivetTypeId == null && q.SerieId == null && q.SystemId == null && q.ThicknessId == null && 
            //    q.WidthId == null && q.WvaDetailsId == null && q.WvaId == null)).ToList();

            //foreach (var item in newItems)
            //{
            //    //BrandId
            //    if (db.Brands.Any(row => row.Name == item.Brand))
            //    {
            //        Brands brand = db.Brands.Where(q => q.Name == item.Brand).SingleOrDefault();
            //        item.BrandId = brand.Id;
            //    }
            //    else
            //    {
            //        var newprod = new Brands();
            //        var nb = db.Brands.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //        int newid = nb.Id + 1;
            //        newprod.Name = item.Brand;
            //        newprod.Id = newid;
            //        item.BrandId = newid;
            //        using (var dbCtx = new BadBeeEntities())
            //        {
            //            dbCtx.Brands.Add(newprod);
            //            dbCtx.SaveChanges();
            //        }
            //    }

            //    //SerieId
            //    if (db.Series.Any(row => row.Name == item.Serie && item.BrandId == row.BrandId))
            //    {
            //        Series serie = db.Series.Where(q => q.Name == item.Serie && item.BrandId == q.BrandId).SingleOrDefault();
            //        item.SerieId = serie.Id;
            //    }
            //    else
            //    {
            //        var newprod = new Series();
            //        var nb = db.Series.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //        int newid = nb.Id + 1;
            //        newprod.Name = item.Serie;
            //        newprod.BrandId = item.BrandId;
            //        newprod.Id = newid;

            //        item.SerieId = newid;
            //        using (var dbCtx = new BadBeeEntities())
            //        {
            //            dbCtx.Series.Add(newprod);
            //            dbCtx.SaveChanges();
            //        }
            //    }
            //    //ModelId
            //    if (db.Models.Any(row => row.Name == item.Model && item.BrandId==row.BrandId && item.SerieId == row.SerieId))
            //    {
            //        Core.DAL.Models model = db.Models.Where(q => q.Name == item.Model && item.BrandId==q.BrandId && item.SerieId == q.SerieId).SingleOrDefault();
            //        item.ModelId = model.Id;
            //    }
            //    else
            //    {
            //        var newprod = new Core.DAL.Models();
            //        var nb = db.Models.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //        int newid = nb.Id + 1;
            //        newprod.Name = item.Model;
            //        newprod.Id = newid;
            //        newprod.BrandId = item.BrandId;
            //        newprod.SerieId = item.SerieId;

            //        item.ModelId = newid;
            //        using (var dbCtx = new BadBeeEntities())
            //        {
            //            dbCtx.Models.Add(newprod);
            //            dbCtx.SaveChanges();
            //        }
            //    }
            //    //WvaId
            //    if (string.IsNullOrEmpty(item.Wva))
            //    {
            //        item.WvaId = 0;
            //    }
            //    else
            //    {
            //        if (db.Wva.Any(row => row.Name == item.Wva))
            //        {
            //            Wva wva = db.Wva.Where(q => q.Name == item.Wva).SingleOrDefault();
            //            item.WvaId = wva.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Wva();
            //            var nb = db.Wva.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.Wva;
            //            newprod.Id = newid;

            //            item.WvaId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Wva.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //WvaDetailsId
            //    if (string.IsNullOrEmpty(item.WvaDetails))
            //    {
            //        item.WvaDetailsId = 0;
            //    }
            //    else
            //    {
            //        if (db.WvaDetails.Any(row => row.Name == item.WvaDetails))
            //        {
            //            WvaDetails wva = db.WvaDetails.Where(q => q.Name == item.WvaDetails).SingleOrDefault();
            //            item.WvaDetailsId = wva.Id;
            //        }
            //        else
            //        {
            //            var newprod = new WvaDetails();
            //            var nb = db.WvaDetails.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.WvaDetails;
            //            newprod.Id = newid;

            //            item.WvaDetailsId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.WvaDetails.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //BadBeeNumberId
            //    if (string.IsNullOrEmpty(item.BadBeeNumber))
            //    {
            //        item.BadBeeNumberId = 0;
            //    }
            //    else
            //    {
            //        if (db.BadBeeNumbers.Any(row => row.BadBeeNumber == item.BadBeeNumber))
            //        {
            //            BadBeeNumbers BadBee = db.BadBeeNumbers.Where(q => q.BadBeeNumber == item.BadBeeNumber).SingleOrDefault();
            //            item.BadBeeNumberId = BadBee.BadBeeNumberId;
            //        }
            //        else
            //        {
            //            var newprod = new BadBeeNumbers();
            //            var nb = db.BadBeeNumbers.OrderByDescending(q => q.BadBeeNumberId).Select(q => q).FirstOrDefault();
            //            int newid = nb.BadBeeNumberId + 1;
            //            newprod.BadBeeNumber = item.BadBeeNumber;
            //            newprod.BadBeeNumberId = newid;

            //            item.BadBeeNumberId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.BadBeeNumbers.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //ThicknessId
            //    if (string.IsNullOrEmpty(item.Thickness))
            //    {
            //        item.ThicknessId = 0;
            //    }
            //    else
            //    {
            //        if (db.Thicknesses.Any(row => row.Name == item.Thickness))
            //        {
            //            Thicknesses thick = db.Thicknesses.Where(q => q.Name == item.Thickness).SingleOrDefault();
            //            item.ThicknessId = thick.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Thicknesses();
            //            var nb = db.Thicknesses.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.Thickness;
            //            newprod.Id = newid;

            //            item.ThicknessId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Thicknesses.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //WidthId
            //    if (string.IsNullOrEmpty(item.Width))
            //    {
            //        item.WidthId = 0;
            //    }
            //    else
            //    {
            //        if (db.Widths.Any(row => row.Name == item.Width))
            //        {
            //            Widths width = db.Widths.Where(q => q.Name == item.Width).SingleOrDefault();
            //            item.WidthId = width.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Widths();
            //            var nb = db.Widths.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.Width;
            //            newprod.Id = newid;

            //            item.WidthId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Widths.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //HeightId
            //    if (string.IsNullOrEmpty(item.Height))
            //    {
            //        item.HeightId = 0;
            //    }
            //    else
            //    {
            //        if (db.Heights.Any(row => row.Name == item.Height))
            //        {
            //            Heights hei = db.Heights.Where(q => q.Name == item.Height).SingleOrDefault();
            //            item.HeightId = hei.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Heights();
            //            var nb = db.Heights.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.Height;
            //            newprod.Id = newid;

            //            item.HeightId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Heights.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //RivetId
            //    if (string.IsNullOrEmpty(item.RivetsType))
            //    {
            //        item.RivetTypeId = 0;
            //    }
            //    else
            //    {
            //        if (db.RivetTypes.Any(row => row.Name == item.RivetsType))
            //        {
            //            RivetTypes rivet = db.RivetTypes.Where(q => q.Name == item.RivetsType).SingleOrDefault();
            //            item.RivetTypeId = rivet.Id;
            //        }
            //        else
            //        {
            //            var newprod = new RivetTypes();
            //            var nb = db.RivetTypes.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.RivetsType;
            //            newprod.Id = newid;

            //            item.RivetTypeId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.RivetTypes.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //SystemId
            //    if (string.IsNullOrEmpty(item.BrakeSystem))
            //    {
            //        item.SystemId = 0;
            //    }
            //    else
            //    {
            //        if (db.Systems.Any(row => row.Name == item.BrakeSystem))
            //        {
            //            Systems system = db.Systems.Where(q => q.Name == item.BrakeSystem).SingleOrDefault();
            //            item.SystemId = system.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Systems();
            //            var nb = db.Systems.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.BrakeSystem;
            //            newprod.Id = newid;

            //            item.SystemId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Systems.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //DrumDiameterId
            //    if (string.IsNullOrEmpty(item.DrumDiameter))
            //    {
            //        item.DrumDiameterId = 0;
            //    }
            //    else
            //    {
            //        if (db.DrumDiameters.Any(row => row.Name == item.DrumDiameter))
            //        {
            //            DrumDiameters drum = db.DrumDiameters.Where(q => q.Name == item.DrumDiameter).SingleOrDefault();
            //            item.DrumDiameterId = drum.Id;
            //        }
            //        else
            //        {
            //            var newprod = new DrumDiameters();
            //            var nb = db.DrumDiameters.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.Name = item.DrumDiameter;
            //            newprod.Id = newid;

            //            item.DrumDiameterId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.DrumDiameters.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }
            //    //PicturesId
            //    if (string.IsNullOrEmpty(item.BadBeeNumber))
            //    {
            //        item.PictureId = 0;
            //    }
            //    else
            //    {
            //        if (db.Pictures.Any(row => row.BadBeeNo == item.BadBeeNumber))
            //        {
            //            Pictures pict = db.Pictures.Where(q => q.BadBeeNo == item.BadBeeNumber).SingleOrDefault();
            //            item.PictureId = pict.Id;
            //        }
            //        else
            //        {
            //            var newprod = new Pictures();
            //            var nb = db.Pictures.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
            //            int newid = nb.Id + 1;
            //            newprod.BadBeeNo = item.BadBeeNumber;
            //            newprod.Picture1 = "";
            //            newprod.Picture2 = "";
            //            newprod.Schema1 = "";
            //            newprod.Schema2 = "";
            //            newprod.Id = newid;

            //            item.PictureId = newid;
            //            using (var dbCtx = new BadBeeEntities())
            //            {
            //                dbCtx.Pictures.Add(newprod);
            //                dbCtx.SaveChanges();
            //            }
            //        }
            //    }

            //    db.Entry(item).State = EntityState.Modified;
            //   db.SaveChanges();
            //}
            

            return RedirectToAction("Index");


            //}
            //catch (Exception e)
            //{
            //    log.Error(e);
            //    throw;
            //}
        }
        public ActionResult GeneratePDF()
        {
            PDFProvider pdfProvider = new PDFProvider();
            pdfProvider.GeneratePDFCatalog();

            return RedirectToAction("Index");
        }
    }
}