using log4net;
using BadBeeAdminPanel.Models;
using BadBee.Core.DAL;
using BadBee.Core.Models;
using BadBee.Core.MyResources;
using BadBee.Core.Providers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BadBeeAdminPanel.Controllers
{
    [Authorize]
    public class PicturesController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(PicturesController));

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

        // GET: Pictures
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetBadBeePadsList()
        {
            try
            {
                List<string> BadBeeNames = new List<string>();
                
                using(BadBeeEntities bbe = new BadBeeEntities())
                {
                    BadBeeNames = bbe.BadBee.Select(q=>q.BadBeeNo).OrderBy(q=>q).ToList();

                }
                return Json(BadBeeNames, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        
        private BadBeeEntities db = new BadBeeEntities();

        public ActionResult FileUpload(HttpPostedFileBase file, string caption, string type, string BadBees)
        {
            try
            {
                Item ifExist = db.Item.Where(q => (q.BadBee.BadBeeNo == BadBees)).FirstOrDefault();
                if (ifExist == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    PicturesModel pictures = new PicturesModel();

                    if (file != null)
                    {
                        if (!string.IsNullOrEmpty(BadBees))
                        {
                            caption = BadBees;
                        }

                        string pic = "";
                        string path = "";

                        if (type == "schema1")
                        {
                            pic = Path.GetFileName(caption + "a.jpg");
                        }
                        else if (type == "schema2")
                        {
                            pic = Path.GetFileName(caption + "b.jpg");
                        }
                        else if (type == "picture2")
                        {
                            pic = Path.GetFileName(caption + "_2.jpg");
                        }
                        else if (type == "picture1")
                        {
                            pic = Path.GetFileName(caption + "_1.jpg");
                        }

                        if (BadBees == null)
                        {
                            BadBees = "";
                        }

                        Picture obj = db.Picture.Where(q => (q.BadBeeNo == BadBees)).FirstOrDefault();

                        if (obj != null)
                        {
                            List<Item> product = db.Item.Where(q => (q.BadBee.BadBeeNo == obj.BadBeeNo)).ToList();

                            //if (type == "schema1" && obj.Schema1 != pic)
                            //{
                            //    obj.Schema1 = pic;
                            //}
                            //if (type == "schema2" && obj.Schema2 != pic)
                            //{
                            //    obj.Schema2 = pic;
                            //}
                            //if (type == "picture2" && obj.Picture2 != pic)
                            //{
                            //    obj.Picture2 = pic;
                            //}
                            //if (type == "picture1" && obj.Picture1 != pic)
                            //{
                            //    obj.Picture1 = pic;
                            //}
                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Entry(obj).State = EntityState.Modified;
                                dbCtx.SaveChanges();
                            }
                        }
                        else
                        {
                            Picture newObj = new Picture();
                            //var np = db.Pictures.OrderByDescending(q => q.Id).Select(q => q).FirstOrDefault();
                            //int newid = np.Id + 1;

                            //newObj.BadBeeNo = BadBees;
                            //if (type == "schema1") { newObj.Schema1 = pic; };
                            //if (type == "schema2") { newObj.Schema2 = pic; };

                            //if (type == "picture1") { newObj.Picture1 = pic; };
                            //if (type == "picture2") { newObj.Picture2 = pic; };
                            //newObj.Id = newid;

                            //List<ItemsDb> product = db.ItemsDb.Where(q => (q.BadBeeNumber == newObj.BadBeeNo)).ToList();
                            //foreach (var item in product)
                            //{
                            //    item.PictureId = newObj.Id;
                            //}

                            using (var dbCtx = new BadBeeEntities())
                            {
                                dbCtx.Picture.Add(newObj);
                                dbCtx.SaveChanges();
                            }
                        }

                        path = Path.Combine(Server.MapPath("~/Images/Pictures"), pic);

                        // file is uploaded

                        file.SaveAs(path);
                        
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                        }

                        db.SaveChanges();
                    }
                    //}
                    // after successfully uploading redirect the user
                    return RedirectToAction("List", pictures);

                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        public ActionResult List(int? page)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                List<string> fileEntries = Directory.GetFiles(Server.MapPath("~/Images/Pictures")).ToList();

                PicturesModel pictures = new PicturesModel();

                var namesList = new List<string>();

                foreach (var item in fileEntries)
                {
                    FileInfo fi = new FileInfo(item);
                    namesList.Add(string.Format(@"../Images/Pictures/{0}", fi.Name));
                }
                int totalCount = namesList.Count;
                namesList = namesList.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderBy(q => q).ToList();

                var pagedList = new StaticPagedList<string>(namesList, pageNumber, pageSize, totalCount);
                pictures.Items = pagedList;

                return View(pictures);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

        public ActionResult Upload()
        {
            try
            {
                string serverPath = Request.QueryString["serverPath"];

                if (!string.IsNullOrEmpty(serverPath))
                {
                    string[] tab = serverPath.Split('/');
                    string name = tab[tab.Length - 1];
                    string filePath = string.Format("{0}\\{1}", Server.MapPath("~/Images/Pictures"), name);
                    string url = "http://badbeecatalog.pl/Upload/Index";

                    using (WebClient client = new WebClient())
                    {
                        client.UploadFile(url, filePath);
                    }


                }

                return RedirectToAction("List");
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }
        public ActionResult Delete(string picturesModel, string BadBeeNo)
        {
            try
            {
                Picture obj = db.Picture.Where(q => (q.BadBeeNo == BadBeeNo)).FirstOrDefault();
                if (obj!=null)
                {
                    if (obj.Picture1==picturesModel)
                    {
                        (from p in db.Picture where p.BadBeeNo == BadBeeNo select p).ToList().ForEach(x =>x.Picture1 ="");
                    }
                    else if (obj.Picture2 == picturesModel)
                    {
                        (from p in db.Picture where p.BadBeeNo == BadBeeNo select p).ToList().ForEach(x => x.Picture2 = "");
                    }
                    else if (obj.Picture3 == picturesModel)
                    {
                        (from p in db.Picture where p.BadBeeNo == BadBeeNo select p).ToList().ForEach(x => x.Picture3 = "");
                    }
                }
                db.SaveChanges();

                string filePath = string.Format("{0}\\{1}", Server.MapPath("~/Images/Pictures"), picturesModel);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return RedirectToAction("List");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}