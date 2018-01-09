using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BadBee.Core.DAL;
using System.Threading;
using System.Globalization;
using log4net;

namespace BadBee.AdminPanel.Controllers
{
    [Authorize]
    public class AspNetUsersController : Controller
    {
        private BadBeeEntities db = new BadBeeEntities();
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(AspNetUsersController));

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
        // GET: AspNetUsers
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }
        

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            aspNetUsers.Removed = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
