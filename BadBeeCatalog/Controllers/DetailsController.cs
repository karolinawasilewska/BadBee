using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BadBeeCatalog.Models;
using System.Globalization;
using System.Threading;
using log4net;
using BadBee.Core.DAL;

namespace BadBeeCatalog.Controllers
{
    public class DetailsController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DetailsController));

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


        // GET: Details
        public ActionResult Index(string id, string date)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "Default");
                }


                using (BadBeeEntities db = new BadBeeEntities())
                {
                //    ItemsDb item = db.Item.Where(q => (q.Id == id)).FirstOrDefault();


                    DetailsModel model = new DetailsModel();
                    //model.BadBeeNumber = item.BadBeeNumber;
                    //model.Wva = item.Wva;
                    //model.Brand = item.Brand;
                    //model.Serie = item.Serie;
                    //model.Models = item.Model;
                    //model.Km = item.Km;
                    //model.Kw = item.Kw;
                    //model.Years = date;
                    //model.Fr = item.Fr;
                    //model.WvaDesc = item.WvaDesc;
                    //model.WvaDetailsQty = item.WvaDetailsQty;
                    //model.WvaDetails = item.WvaDetails;
                    //model.Wedge = item.Wedge;
                    //model.DrumDiameter = item.DrumDiameter;
                    //model.RivetsQuantity = item.RivetsQuantity;
                    //model.RivetsType = item.RivetsType;
                    //model.System = item.BrakeSystem;
                    //model.Width = item.Width;
                    //model.Height = item.Height;
                    //model.Thickness = item.Thickness;
                    ////model.Schema1 = item.Schema1;
                    ////model.Schema2 = item.Schema2;
                    ////model.Schema3 = item.Schema3;
                    ////model.Picture1 = item.Picture1;
                    ////model.Picture2 = item.Picture2;
                    //model.PictureId = item.PictureId;
                    //model.Type = item.ProductType;

                    String path = Server.MapPath(@"Images/Pictures/");

                    if (path.Contains("\\Details\\Index"))
                    {
                        path = path.Replace("\\Details\\Index", "");
                    }
                    List<string> pictures = new List<string>();
                    List<string> badBeeNoList = new List<string>();
                    List<string> wvaFromBadBeeNoList = new List<string>();
                    List<string> wvaList = new List<string>();

                    if (model.Schema1=="lining.bmp"|| model.Type=="Brake lining") //okładzina
                    {
                        pictures.Add("559 A.jpg");
                    }
                    else //klocek
                    {
                        if (model.Wva!="ND")
                        {
                            pictures = FindPicture(model, path);
                        }
                        if (pictures.Count==0 || model.Wva=="ND")
                        {
                            pictures.Add("noimg.jpg");
                        }

                    }
                    model.Pictures = pictures.ToArray();
                    model.Picture = model.Pictures.First();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
        public List<string> FindPicture(DetailsModel model, string path)
        {
            List<string> pictures = new List<string>();

            using (BadBeeEntities db = new BadBeeEntities())
            {
                // Pictures pict = db.Pictures.Where(q => q.BadBeeNo == model.BadBeeNumber).FirstOrDefault();
                //if (pict != null)
                //{

                //    if (!string.IsNullOrEmpty(pict.Schema1) && System.IO.File.Exists(path + pict.Schema1))
                //    {
                //        pictures.Add(pict.Schema1);
                //    }
                //    if (!string.IsNullOrEmpty(pict.Schema2) && System.IO.File.Exists(path + pict.Schema2))
                //    {
                //        pictures.Add(pict.Schema2);
                //    }
                //    if (!string.IsNullOrEmpty(pict.Picture2) && System.IO.File.Exists(path + pict.Picture2))
                //    {
                //        pictures.Add(pict.Picture2);
                //    }
                //    if (!string.IsNullOrEmpty(pict.Picture1) && System.IO.File.Exists(path + pict.Picture1))
                //    {
                //        pictures.Add(pict.Picture1);
                //    }
                //}
                //return pictures.Distinct().ToList();
                return null;
            }
        }
    }

    }