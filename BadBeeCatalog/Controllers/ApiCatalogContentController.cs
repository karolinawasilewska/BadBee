using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BadBeeCatalog.Models;
using PagedList;
using Newtonsoft;
using Newtonsoft.Json;


namespace BadBeeCatalog.Controllers
{
    public class ApiCatalogContentController : Controller
    {
        public static int ITEM_PER_PAGE = 20;

        // GET: /ApiCatalogContent/ItemsList
        //public JsonResult ItemsList(
        //    string brand,
        //    string serie,
        //    string model,
        //    short? dateYear,
        //    short? dateMonth,
        //    string attributeId,
        //    String wva,
        //    string width,
        //    string height,
        //    string thickness,
        //    String rivet,
        //    double? size,
        //    int? page)
        //{
        //    ApiItemsListResult result = new ApiItemsListResult();
        //    int pageNumber = page ?? 1;
        //    BadBeeFilter filter = new BadBeeFilter()
        //    {
        //        Brand = brand,
        //        Serie = serie,
        //        Model = model,
        //        DateYear = dateMonth,
        //        DateMonth = dateMonth,
        //        AttributeId = attributeId,
        //        Wva = wva,
        //        Width = width,
        //        Height = height,
        //        Thickness = thickness,
        //        Rivet = rivet,
        //        Size = size
        //    };

        //    using (ListProvider provider = new ListProvider())
        //    {
        //        //BadBeeCatalog.Providers.ListProvider.ListResult listResult = provider.GetList(filter, pageNumber, ITEM_PER_PAGE);
        //        //result.Items = listResult.Items;
        //        //result.ItemsCount = listResult.ItemsCount;
        //    }

        //    return Json(new ApiJsonResult(result), JsonRequestBehavior.AllowGet);
        //}


        //// GET: /ApiCatalogContent/Details
        //public JsonResult Details(Guid? badBeeId)
        //{
        //    ApiDetailsResult result = null;
        //    ItemsDb obj = null;
        //    String picture = "";

        //    if (badBeeId.HasValue && badBeeId.Value != Guid.Empty)
        //    {
        //        using (ListProvider provider = new ListProvider())
        //        {
        //          //  obj = provider.GetDetails(badBeeId.Value);
        //        }

               
        //            picture = "Lin.jpg";
                               

        //        if (obj != null)
        //        {
        //            result = new ApiDetailsResult()
        //            {
        //                Id = obj.BadBeeNumberId,
        //                BadBeeId = obj.BadBeeNumber,
        //                Image = picture
        //            };
        //        }

        //        return Json(new ApiJsonResult(result), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new ApiJsonResult(false, "Empty badBeeId parameter"), JsonRequestBehavior.AllowGet);
        //    }
        //}

    }
}
