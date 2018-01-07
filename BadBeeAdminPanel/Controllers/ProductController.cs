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
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
                
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
            var product = db.Item.Where(q=>q.Id==id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);

            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        public ActionResult Create()
        {
            return View(new ProductModels());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item product, string returnUrl = null)
        {
            try
            {
            if (ModelState.IsValid)
            {
                string logoPath = "";
                string logoRelativePath = "";
               
                var modified = db.Item.Where(s => s.Id == product.Id).FirstOrDefault();

                    //if (modified != null)
                    //{
                    //    if (!string.IsNullOrEmpty(product.Model.Serie.Brand.Name))
                    //    {
                    //        modified.Model.Serie.Brand = product.Model.Serie.Brand;

                    //        if (db.Brand.Any(row => row.BrandId == product.Model.Serie.Brand.BrandId))
                    //        {
                    //            Brand br = db.Brand.Where(q => q.BrandId == product.Model.Serie.Brand.BrandId).SingleOrDefault();
                    //            if (br.BrandId != product.Model.Serie.BrandId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.BrandId = br.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.BrandId = product.BrandId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Brands();
                    //            var nb = db.Brands.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Brand;
                    //            newprod.Id = newid;
                    //            modified.BrandId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Brands.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.Brand = "";
                    //        //  modified.BrandId = 0;
                    //    }

                    //    if (!string.IsNullOrEmpty(modified.Type)) { modified.Type = product.Type; } else { modified.Type = ""; }

                    //    if (product.Serie == null)
                    //    {
                    //        modified.Serie = "";
                    //        if (db.Series.Any(row => row.BrandId == modified.BrandId && row.Name == ""))
                    //        {
                    //            //seria pusta, brand pustą serią jest już w bazie
                    //            Series ser = db.Series.Where(q => q.BrandId == modified.BrandId).SingleOrDefault();
                    //            modified.SerieId = ser.Id;
                    //        }
                    //        else
                    //        {
                    //            //seria pusta, brandu z pustą serią nie ma w bazie
                    //            var newproduct = new Series();
                    //            var np = db.Series.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = np.Id + 1;
                    //            newproduct.Name = "";
                    //            newproduct.Id = newid;
                    //            newproduct.BrandId = modified.BrandId;
                    //            modified.SerieId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Series.Add(newproduct);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.Serie = product.Serie;
                    //        if (db.Series.Any(row => row.Name == product.Serie))
                    //        {
                    //            Series ser = db.Series.Where(q => q.Name == product.Serie).SingleOrDefault();
                    //            if (ser.Id != product.SerieId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.SerieId = ser.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.SerieId = product.SerieId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Series();
                    //            var nb = db.Series.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Serie;
                    //            newprod.Id = newid;
                    //            newprod.BrandId = modified.BrandId;
                    //            modified.SerieId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Series.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }

                    //    if (product.Model == null)
                    //    {
                    //        modified.Model = "";

                    //        if (db.Models.Any(row => row.SerieId == modified.SerieId && row.Name == ""))
                    //        {
                    //            Core.DAL.Models mod = db.Models.Where(q => q.SerieId == modified.SerieId).SingleOrDefault();
                    //            modified.ModelId = mod.Id;
                    //        }
                    //        else
                    //        {
                    //            //model pusty, serii z pustym modelem nie ma w bazie
                    //            var newproduct = new Core.DAL.Models();
                    //            var np = db.Models.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = np.Id + 1;
                    //            newproduct.Name = "";
                    //            newproduct.Id = newid;
                    //            newproduct.SerieId = modified.SerieId;
                    //            newproduct.BrandId = modified.BrandId;
                    //            modified.ModelId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Models.Add(newproduct);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                        
                    //    else
                    //    {
                    //        modified.Model = product.Model;
                    //        if (db.Models.Any(row => row.Name == product.Model))
                    //        {
                    //            Core.DAL.Models mod = db.Models.Where(q => q.Name == product.Model && q.SerieId==modified.SerieId).SingleOrDefault();
                    //            if (mod.Id != product.ModelId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.ModelId = mod.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.ModelId = product.ModelId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Core.DAL.Models();
                    //            var nb = db.Models.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Model;
                    //            newprod.Id = newid;
                    //            newprod.BrandId = modified.BrandId;
                    //            newprod.SerieId = modified.SerieId;
                    //            modified.ModelId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Models.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(product.Fr)) { modified.Fr = product.Fr; } else { modified.Fr = ""; }

                       
                    //modified.DateFrom = product.DateFrom;
                    //modified.DateTo = product.DateTo;


                    //    if (!string.IsNullOrEmpty(product.WvaDesc)) { modified.WvaDesc = product.WvaDesc; } else { modified.WvaDesc = ""; }

                    //    if (!string.IsNullOrEmpty(product.Wva))
                    //    {
                    //        modified.Wva = product.Wva;
                    //        if (db.Wva.Any(row => row.Name == product.Wva))
                    //        {
                    //            Wva wva = db.Wva.Where(q => q.Name == product.Wva).SingleOrDefault();
                    //            if (wva.Id != product.WvaId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.WvaId = wva.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.WvaId = product.WvaId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Wva();
                    //            var nb = db.Wva.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Wva;
                    //            newprod.Id = newid;
                    //            modified.WvaId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Wva.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.Wva = "";
                    //        modified.WvaId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.WvaDetailsQty)) { modified.WvaDetailsQty = product.WvaDetailsQty; } else { modified.WvaDetailsQty = ""; }

                    //    if (!string.IsNullOrEmpty(product.WvaDetails))
                    //    {
                    //        modified.WvaDetails = product.WvaDetails;
                    //        if (db.WvaDetails.Any(row => row.Name == product.WvaDetails))
                    //        {
                    //            WvaDetails wva = db.WvaDetails.Where(q => q.Name == product.WvaDetails).SingleOrDefault();
                    //            if (wva.Id != product.WvaDetailsId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.WvaDetailsId = wva.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.WvaDetailsId = product.WvaDetailsId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new WvaDetails();
                    //            var nb = db.WvaDetails.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.WvaDetails;
                    //            newprod.Id = newid;
                    //            modified.WvaDetailsId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.WvaDetails.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.WvaDetails = "";
                    //        modified.WvaDetailsId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.BadBeeNumber))
                    //    {
                    //        modified.BadBeeNumber = product.BadBeeNumber;
                    //        if (db.BadBeeNumbers.Any(row => row.BadBeeNumber == product.BadBeeNumber))
                    //        {
                    //            BadBeeNumbers bb = db.BadBeeNumbers.Where(q => q.BadBeeNumber == product.BadBeeNumber).SingleOrDefault();
                    //            if (bb.BadBeeNumberId != product.BadBeeNumberId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.BadBeeNumberId = bb.BadBeeNumberId;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.BadBeeNumberId = product.BadBeeNumberId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new BadBeeNumbers();
                    //            var nb = db.BadBeeNumbers.OrderByDescending(q => q.BadBeeNumberId).Select(q => q).FirstOrDefault();
                    //            int newid = nb.BadBeeNumberId + 1;
                    //            newprod.BadBeeNumber = product.BadBeeNumber;
                    //            newprod.BadBeeNumberId = newid;
                    //            modified.WvaDetailsId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.BadBeeNumbers.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.BadBeeNumber = "";
                    //        modified.BadBeeNumberId = 0;
                    //    }
                    //    //if (!string.IsNullOrEmpty(product.Size)) { modified.Size = product.Size; } else { modified.Size = ""; }

                    //    if (!string.IsNullOrEmpty(product.Height))
                    //    {
                    //        modified.Height = product.Height;
                    //        if (db.Heights.Any(row => row.Name == product.Height))
                    //        {
                    //            Heights hei = db.Heights.Where(q => q.Name == product.Height).SingleOrDefault();
                    //            if (hei.Id != product.HeightId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.HeightId = hei.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.HeightId = product.HeightId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Heights();
                    //            var nb = db.Heights.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Height;
                    //            newprod.Id = newid;
                    //            modified.HeightId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Heights.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.Height = "";
                    //        modified.HeightId = 0;
                    //    }

                    //    if (!string.IsNullOrEmpty(product.Width))
                    //    {
                    //        modified.Width = product.Width;

                    //        if (db.Widths.Any(row => row.Name == product.Width))
                    //        {
                    //            Widths wid = db.Widths.Where(q => q.Name == product.Width).SingleOrDefault();
                    //            if (wid.Id != product.WidthId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.WidthId = wid.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.WidthId = product.WidthId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Widths();
                    //            var nb = db.Widths.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Width;
                    //            newprod.Id = newid;
                    //            modified.WidthId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Widths.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else {
                    //        modified.Width = "";
                    //        modified.WidthId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.Thickness))
                    //    {
                    //        modified.Thickness = product.Thickness;
                    //        if (db.Thicknesses.Any(row => row.Name == product.Thickness))
                    //        {
                    //            Thicknesses th = db.Thicknesses.Where(q => q.Name == product.Thickness).SingleOrDefault();
                    //            if (th.Id != product.ThicknessId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.ThicknessId = th.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.ThicknessId = product.ThicknessId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Thicknesses();
                    //            var nb = db.Thicknesses.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.Thickness;
                    //            newprod.Id = newid;
                    //            modified.ThicknessId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Thicknesses.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.Thickness = "";
                    //        modified.ThicknessId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.Wedge)) { modified.Wedge = product.Wedge; } else { modified.Wedge = ""; }

                    //    if (!string.IsNullOrEmpty(product.DrumDiameter))
                    //    {
                    //        modified.DrumDiameter = product.DrumDiameter;
                    //        if (db.DrumDiameters.Any(row => row.Name == product.DrumDiameter))
                    //        {
                    //            DrumDiameters dia = db.DrumDiameters.Where(q => q.Name == product.DrumDiameter).SingleOrDefault();
                    //            if (dia.Id != product.DrumDiameterId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.DrumDiameterId = dia.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.DrumDiameterId = product.DrumDiameterId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new DrumDiameters();
                    //            var nb = db.DrumDiameters.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.DrumDiameter;
                    //            newprod.Id = newid;
                    //            modified.DrumDiameterId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.DrumDiameters.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.DrumDiameter = "";
                    //        modified.DrumDiameterId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.BrakeSystem))
                    //    {
                    //        modified.BrakeSystem = product.BrakeSystem;
                    //        if (db.Systems.Any(row => row.Name == product.BrakeSystem))
                    //        {
                    //            Systems sys = db.Systems.Where(q => q.Name == product.BrakeSystem).SingleOrDefault();
                    //            if (sys.Id != product.SystemId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.SystemId = sys.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.SystemId = product.SystemId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new Systems();
                    //            var nb = db.Systems.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.BrakeSystem;
                    //            newprod.Id = newid;
                    //            modified.SystemId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.Systems.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.BrakeSystem = "";
                    //        modified.SystemId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.RivetsQuantity)) { modified.RivetsQuantity = product.RivetsQuantity; } else { modified.RivetsQuantity = ""; }

                    //    if (!string.IsNullOrEmpty(product.RivetsType))
                    //    {
                    //        modified.RivetsType = product.RivetsType;
                    //        if (db.RivetTypes.Any(row => row.Name == product.RivetsType))
                    //        {
                    //            RivetTypes riv = db.RivetTypes.Where(q => q.Name == product.RivetsType).SingleOrDefault();
                    //            if (riv.Id != product.RivetTypeId)
                    //            {
                    //                //jest w bazie z innym id
                    //                modified.RivetTypeId = riv.Id;
                    //            }
                    //            else
                    //            {
                    //                //jest w bazie z dobrym id
                    //                modified.RivetTypeId = product.RivetTypeId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //nie ma w bazie
                    //            var newprod = new RivetTypes();
                    //            var nb = db.RivetTypes.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    //            int newid = nb.Id + 1;
                    //            newprod.Name = product.RivetsType;
                    //            newprod.Id = newid;
                    //            modified.RivetTypeId = newid;
                    //            using (var dbCtx = new BadBeeEntities())
                    //            {
                    //                dbCtx.RivetTypes.Add(newprod);
                    //                dbCtx.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        modified.RivetsType = "";
                    //        modified.RivetTypeId = 0;
                    //    }
                    //    if (!string.IsNullOrEmpty(product.ProductType)) { modified.ProductType = product.ProductType; } else { modified.ProductType = ""; }

                      

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

                return View(product);
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
                string logoPath = "";
                string logoRelativePath = "";
               
                Item newProd = new Item();

                    var newProductId = db.Item.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                    newProd.Id = newProductId.Id + 1;

                    //model - wymagany
                    newProd.Model = new Model();
                    newProd.Model.Serie = new Serie();
                    newProd.Model.Serie.Brand = new Brand();
                    newProd.BadBee = new BadBee.Core.DAL.BadBee();


                    newProd.Model.Name = model.ModelName;

                    if (db.Model.Any(row => row.Name == model.ModelName))
                    {
                        Model mod = db.Model.Where(q => q.Name == newProd.Model.Name).SingleOrDefault();
                        //jest w bazie 
                        newProd.ModelId = mod.ModelId;
                    }
                    else
                    {
                        //nie ma w bazie
                        var newproduct = new Model();
                        var np = db.Model.OrderByDescending(q => q.ModelId).Select(q => q).FirstOrDefault();
                        int newid = np.ModelId + 1;
                        newproduct.Name = model.ModelName;
                        newproduct.ModelId = newid;
                        newproduct.SerieId = newProd.Model.SerieId;

                        newProd.ModelId = newid;
                        //using (var dbCtx = new BadBeeEntities())
                        //{
                        //    dbCtx.Model.Add(newproduct);
                        //    dbCtx.SaveChanges();
                        //}
                    }

                    //seria - niewymagana
                    newProd.Model.Serie.Name = model.Serie;

                    if (model.Serie == null)
                    {
                        newProd.Model.Serie.Name = "";
                        if (db.Serie.Any(row => row.BrandId == newProd.Model.Serie.BrandId && row.Name == ""))
                        {
                            //seria pusta, brand pustą serią jest już w bazie
                            Serie ser = db.Serie.Where(q => q.BrandId == newProd.Model.Serie.BrandId).SingleOrDefault();
                            newProd.Model.Serie.SerieId = ser.SerieId;
                        }
                        else
                        {
                            //seria pusta, brandu z pustą serią nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = "";
                            newproduct.SerieId = newid;
                            newproduct.BrandId = newProd.Model.Serie.Brand.BrandId;
                            newProd.Model.Serie.SerieId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Serie.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                    }
                    else
                    {
                        if (db.Serie.Any(row => row.Name == model.Serie))
                        {
                            //jest w bazie 
                            Serie ser = db.Serie.Where(q => q.Name == newProd.Model.Serie.Name).SingleOrDefault();
                            newProd.Model.Serie.SerieId = ser.SerieId;

                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Serie();
                            var np = db.Serie.OrderByDescending(q => q.SerieId).Select(q => q).FirstOrDefault();
                            int newid = np.SerieId + 1;
                            newproduct.Name = model.Serie;
                            newproduct.SerieId = newid;
                            newproduct.BrandId = newProd.Model.Serie.Brand.BrandId;
                            newProd.Model.Serie.SerieId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Serie.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                    }

                    //brand - wymagany

                    if (!string.IsNullOrEmpty(model.Brand))
                    {
                        newProd.Model.Serie.Brand.Name = model.Brand;

                        if (db.Brand.Any(row => row.Name == model.Brand))
                        {
                            Brand br = db.Brand.Where(q => q.Name == newProd.Model.Serie.Brand.Name).SingleOrDefault();
                            //jest w bazie 
                            newProd.Model.Serie.Brand.BrandId = br.BrandId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newBrand = new Brand();
                            var nb = db.Brand.OrderByDescending(q => q.BrandId).Select(q => q).FirstOrDefault();
                            int newid = nb.BrandId + 1;
                            newBrand.Name = model.Brand;
                            newBrand.BrandId = newid;
                            newProd.Model.Serie.Brand.BrandId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Brand.Add(newBrand);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                    }
                    
                   
                    
                  
                    if (!string.IsNullOrEmpty(model.Fr)) { newProd.BadBee.FR = model.Fr; } else { newProd.BadBee.FR = ""; }

                    //if (model.DateFrom != null)
                    //{
                    //    newProd.DateFrom = model.DateFrom;
                    //}
                    //if (model.DateTo != null)
                    //{
                    //    newProd.DateTo = model.DateTo;
                    //}

                    
                    if (!string.IsNullOrEmpty(model.BadBeeNumber))
                    {
                        newProd.BadBee.BadBeeNo = model.BadBeeNumber;
                        if (db.BadBee.Any(row => row.BadBeeNo == model.BadBeeNumber))
                        {
                            BadBee.Core.DAL.BadBee bb = db.BadBee.Where(q => q.BadBeeNo == newProd.BadBee.BadBeeNo).Where(q=>q.FR==newProd.BadBee.FR).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBeeId = bb.BadBeeId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new BadBee.Core.DAL.BadBee();
                            var np = db.BadBee.OrderByDescending(q => q.BadBeeId).Select(q => q).FirstOrDefault();
                            int newid = np.BadBeeId + 1;
                            newproduct.BadBeeNo = model.BadBeeNumber;
                            newproduct.BadBeeId = newid;
                            newProd.BadBeeId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.BadBeeNumbers.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                    }
                    //else
                    //{
                    //    newProd.BadBeeNumberId = 0;
                    //    newProd.BadBeeNumber = "";
                    //}

                    newProd.BadBee.Wva = new Wva();

                    if (!string.IsNullOrEmpty(model.WvaDesc)) { newProd.BadBee.Wva.Description = model.WvaDesc; } else { newProd.BadBee.Wva.Description = ""; }

                    if (!string.IsNullOrEmpty(model.Wva))
                    {
                        newProd.BadBee.Wva.WvaNo = model.Wva;
                        if (db.Wva.Any(row => row.WvaNo == model.Wva))
                        {
                            Wva wva = db.Wva.Where(q => q.WvaNo == newProd.BadBee.Wva.WvaNo).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBee.WvaId = wva.WvaId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Wva();
                            var np = db.Wva.OrderByDescending(q => q.WvaId).Select(q => q).FirstOrDefault();
                            int newid = np.WvaId + 1;
                            newproduct.WvaNo = model.Wva;
                            newproduct.WvaId = newid;
                            newProd.BadBee.WvaId = newid;
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Wva.Add(newproduct);
                                dbCtx.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        newProd.BadBee.WvaId = 0;
                        //newProd.Wva = "";
                    }


                    newProd.BadBee.Dimension = new Dimension();
                    newProd.BadBee.Dimension.Height = new Height();
                    newProd.BadBee.Dimension.Width = new Width();
                    newProd.BadBee.Dimension.Thickness = new Thickness();

                    newProd.BadBee.DimensionId = 0;
                    newProd.BadBee.SystemId = 0;

                  

                    
                    
                        newProd.BadBee.Dimension.Height.Height1 = model.Height;
                        if (db.Height.Any(row => row.Height1 == model.Height))
                        {
                            Height hei = db.Height.Where(q => q.Height1 == newProd.BadBee.Dimension.Height.Height1).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBee.Dimension.HeightId = hei.HeightId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Height();
                            var np = db.Height.OrderByDescending(q => q.HeightId).Select(q => q).FirstOrDefault();
                            int newid = np.HeightId + 1;
                            newproduct.Height1 = model.Height;
                            newproduct.HeightId = newid;
                            newProd.BadBee.Dimension.Height.HeightId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Heights.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                   

                   
                        newProd.BadBee.Dimension.Width.Width1 = model.Width;
                        if (db.Width.Any(row => row.Width1 == model.Width))
                        {
                            Width wid = db.Width.Where(q => q.Width1 == newProd.BadBee.Dimension.Width.Width1).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBee.Dimension.WidthId = wid.WidthId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Width();
                            var np = db.Width.OrderByDescending(q => q.WidthId).Select(q => q).FirstOrDefault();
                            int newid = np.WidthId + 1;
                            newproduct.Width1 = model.Width;
                            newproduct.WidthId = newid;
                            newProd.BadBee.Dimension.WidthId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Widths.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }

                        newProd.BadBee.Dimension.Thickness.Thickness1 = model.Thickness;
                        if (db.Thickness.Any(row => row.Thickness1 == model.Thickness))
                        {
                            Thickness th = db.Thickness.Where(q => q.Thickness1 == newProd.BadBee.Dimension.Thickness.ThicknessId).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBee.Dimension.ThicknessId = th.ThicknessId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Thickness();
                            var np = db.Thickness.OrderByDescending(q => q.ThicknessId).Select(q => q).FirstOrDefault();
                            int newid = np.ThicknessId + 1;
                            newproduct.Thickness1 = model.Thickness;
                            newproduct.ThicknessId = newid;
                            newProd.BadBee.Dimension.ThicknessId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Thicknesses.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                    }

                    if (!string.IsNullOrEmpty(model.BrakeSystem))
                    {
                        newProd.BadBee.Systems.Abbreviation = model.BrakeSystem;
                        if (db.Systems.Any(row => row.Abbreviation == model.BrakeSystem))
                        {
                            Systems sys = db.Systems.Where(q => q.Abbreviation == newProd.BadBee.Systems.Abbreviation).SingleOrDefault();
                            //jest w bazie 
                            newProd.BadBee.SystemId = sys.SystemId;
                        }
                        else
                        {
                            //nie ma w bazie
                            var newproduct = new Systems();
                            var np = db.Systems.OrderByDescending(q => q.SystemId).Select(q => q).FirstOrDefault();
                            int newid = np.SystemId + 1;
                            newproduct.Abbreviation = model.BrakeSystem;
                            newproduct.SystemId = newid;
                            newProd.BadBee.SystemId = newid;
                            //using (var dbCtx = new BadBeeEntities())
                            //{
                            //    dbCtx.Systems.Add(newproduct);
                            //    dbCtx.SaveChanges();
                            //}
                        }
                    }
                    else
                    {
                        newProd.BadBee.SystemId = 0;
                    }

                    db.Item.Add(newProd);
                    db.SaveChanges();

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
                return Json(new SelectList(brands, "Id", "Name"), JsonRequestBehavior.AllowGet);
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

                return Json(new SelectList(series, "Id", "Name"), JsonRequestBehavior.AllowGet);
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

                return Json(new SelectList(models, "Id", "Name"), JsonRequestBehavior.AllowGet);
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

                return Json(new SelectList(years, "DateId", "Date"), JsonRequestBehavior.AllowGet);
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
                return Json(new SelectList(wva, "Id", "Name"), JsonRequestBehavior.AllowGet);
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
                return Json(new SelectList(widths, "Id", "Name"), JsonRequestBehavior.AllowGet);
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
                return Json(new SelectList(heights, "Id", "Name"), JsonRequestBehavior.AllowGet);
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
                return Json(new SelectList(thicknesses, "Id", "Name"), JsonRequestBehavior.AllowGet);
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
                return Json(new SelectList(systems, "Id", "Name"), JsonRequestBehavior.AllowGet);
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

                return Json(new SelectList(BadBees, "BadBeeNumberId", "BadBeeNumber"), JsonRequestBehavior.AllowGet);
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