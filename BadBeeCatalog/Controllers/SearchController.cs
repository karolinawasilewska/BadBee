using log4net;
using BadBeeCatalog.Models;
using BadBee.Core.DAL;
using BadBee.Core.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BadBeeCatalog.Controllers
{
    public class SearchController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DefaultController));
        //
        // GET: /Search/
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
        public ActionResult Index()
        {
            string brandId = "";
            string serieId = "";
            string modelId = "";
            try
            {
                ListProvider listProvider = new ListProvider();
                SearchModel model = null;

                brandId = Request.QueryString["brand"];
                serieId = Request.QueryString["serie"];
                modelId = Request.QueryString["model"];

                if (string.IsNullOrEmpty(brandId))
                {
                    model = CreateSearchModel(listProvider.GetChList<Brand>().ToList());
                }
                else if (string.IsNullOrEmpty(serieId))
                {
                    model = CreateSearchModel(listProvider.GetChList<Serie>().Where(q => q.BrandId.ToString() == brandId).ToList());
                    //if (model.Columns[0].Rows.Count == 1 && string.IsNullOrEmpty(model.Columns[0].Rows[0].Name))
                    //{
                    //    return RedirectToAction("Index", "Search", model.Columns[0].Rows[0].CustomParams);
                    //}
                }
                else if (string.IsNullOrEmpty(modelId))
                {
                    model = CreateSearchModel(listProvider.GetChList<Model>().Where(q => q.SerieId.ToString() == serieId).ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Default", new { arg1 = brandId, arg2 = serieId, arg3 = modelId });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public ActionResult Crosses()
        {
            try
            {
                ListProvider listProvider = new ListProvider();
                SearchModel model = null;
                                
                //string brandId = Request.QueryString["crossBrand"];

                //if (string.IsNullOrEmpty(brandId))
                //{
                //    model = CreateSearchModel(listProvider.GetChList<Crosses>().Select(q => q.CrossBrandName).Distinct().OrderBy(q => q).ToList());
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Default", new { arg4 = Request.QueryString["crossBrand"] });

                //}

                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }



        public SearchModel CreateSearchModel<T>(List<T> data)
        {
            try
            {
                SearchModel model = new SearchModel();
                int columnsCount = 4;
                model.Columns = new List<SearchColumnModel>();
                for (int i = 0; i < columnsCount; i++)
                {
                    model.Columns.Add(new SearchColumnModel() { Rows = new List<SearchRowModel>() });
                }

                int count = data.Count();
                int itemsPerColumn;
                if (count % 4 == 0)
                    itemsPerColumn = count / columnsCount;
                else
                    itemsPerColumn = (count / columnsCount) + 1;

                int columnIndex = 0;
                for (int i = 0; i < count; i++)
                {
                    if (model.Columns[columnIndex].Rows != null && model.Columns[columnIndex].Rows.Count == itemsPerColumn)
                    {
                        columnIndex++;
                    }

                    var item = data[i];
                    var aa = item.GetType().BaseType;
                    if (item.GetType().BaseType == typeof(Brand))
                    {
                        var brand = item as Brand;
                        model.Columns[columnIndex].Rows.Add(new SearchRowModel() { CustomParams = new { brand = brand.BrandId, brandName = brand.Name }, Name = brand.Name });
                    }
                    //if (item.GetType().BaseType == typeof(string))
                    //{
                    //    var crossBrand = item as string;
                    //    model.Columns[columnIndex].Rows.Add(new SearchRowModel() { CustomParams = new { crossBrand = crossBrand }, Name = crossBrand });
                    //}
                    else if (item.GetType().BaseType == typeof(Serie))
                    {
                        var serie = item as Serie;
                        model.Columns[columnIndex].Rows.Add(new SearchRowModel() { CustomParams = new { brand = Request.QueryString["brand"], brandName = Request.QueryString["brandName"], serie = serie.SerieId, serieName = serie.Name }, Name = serie.Name });
                    }
                    else if (item.GetType().BaseType == typeof(Model))
                    {
                        var truckModel = item as Model;
                        model.Columns[columnIndex].Rows.Add(new SearchRowModel()
                        {
                            CustomParams = new
                            {
                                brand = Request.QueryString["brand"],
                                serie = Request.QueryString["serie"],
                                model = truckModel.ModelId
                            },
                            Name = truckModel.Name
                        });
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }

        }
    }
}
