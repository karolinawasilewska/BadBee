using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BadBeeCatalog.Controllers
{
    public class UploadController : Controller
    {

        public ActionResult Index(HttpPostedFileBase file, string pictName)
        {

            if (file != null)
            {
                string path = Path.Combine(Server.MapPath("~/Images/Pictures"), file.FileName);
                


                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }
            return RedirectToAction("Index", "Default");
        }
        // GET: Upload
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}