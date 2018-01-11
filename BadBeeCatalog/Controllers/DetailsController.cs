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
        public ActionResult Index(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "Default");
                }


                using (BadBeeEntities db = new BadBeeEntities())
                {
                    Item item = db.Item.Where(q => q.Id == id).FirstOrDefault();

                    if (item != null)
                    {
                        DetailsModel model = new DetailsModel();
                        model.BadBeeNumber = item.BadBee.BadBeeNo;
                        model.Wva = item.BadBee.Wva.WvaNo;
                        model.Brand = item.Model.Serie.Brand.Name;
                        model.Serie = item.Model.Serie.Name;
                        model.Models = item.Model.Name;
                        model.Years = GetYears(item.Model.Year.DateFromFK.Date1, item.Model.Year.DateToFK.Date1);
                        model.Fr = item.BadBee.FR;
                        model.WvaDesc = item.BadBee.Wva.Description;
                        model.System = item.BadBee.Systems.Abbreviation;
                        model.Width = item.BadBee.Dimension.Width.Width1.ToString();
                        model.Height = item.BadBee.Dimension.Height.Height1.ToString();
                        model.Thickness = item.BadBee.Dimension.Thickness.Thickness1.ToString();
                        model.PictureId = item.BadBee.PictureId;

                        String path = Server.MapPath(@"Image/Pictures/");

                        if (path.Contains("\\Details\\Index"))
                        {
                            path = path.Replace("\\Details\\Index", "");
                        }
                        List<string> pictures = new List<string>();
                        List<string> badBeeNoList = new List<string>();
                        List<string> wvaFromBadBeeNoList = new List<string>();
                        List<string> wvaList = new List<string>();


                        if (model.PictureId == 0)
                        {
                            pictures.Add("noimg.jpg");
                        }
                        else
                        {
                            pictures = FindPicture(model, path);
                            if (pictures.Count()==0)
                            {
                                pictures.Add("noimg.jpg");
                            }
                        }
                        model.Pictures = pictures.ToArray();
                        model.Picture = model.Pictures.First();
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Default");
                    }
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
                Picture pict = db.Picture.Where(q => q.BadBeeNo == model.BadBeeNumber).FirstOrDefault();
                if (pict != null)
                {

                    if (!string.IsNullOrEmpty(pict.Picture1) && System.IO.File.Exists(path + pict.Picture1))
                    {
                        pictures.Add(pict.Picture1);
                    }
                    if (!string.IsNullOrEmpty(pict.Picture2) && System.IO.File.Exists(path + pict.Picture2))
                    {
                        pictures.Add(pict.Picture2);
                    }
                    if (!string.IsNullOrEmpty(pict.Picture3) && System.IO.File.Exists(path + pict.Picture3))
                    {
                        pictures.Add(pict.Picture3);
                    }
                                   }
                return pictures.Distinct().ToList();
            }
        }
        public string GetYears(string dateFrom, string dateTo)
        {
            if (dateFrom == "0" && dateTo == "0")
            {
                return string.Empty;
            }
            else if (dateFrom != "0" && dateTo == "0")
            {
                return string.Format(dateFrom + "->");
            }
            else if (dateFrom == "0" && dateTo != "0")
            {
                return string.Format("->" + dateTo);
            }
            else
            {
                return string.Format(dateFrom + "->" + dateTo);
            }

        }
    }

    }