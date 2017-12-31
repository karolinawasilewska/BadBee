using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BadBee.Core.DAL;
using log4net;
using System.Globalization;
using System.Threading;
using BadBee.Core.Models;

namespace BadBeeAdminPanel.Controllers
{
    [Authorize]
    public class WvasController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(WvasController));

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
        private BadBeeEntities db = new BadBeeEntities();

        // GET: Wvas
        public ActionResult Index()
        {
            try
            {
            return View(db.Wva.OrderBy(q=>q.WvaNo).ToList());
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

      

        // GET: Wvas/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wva wva = db.Wva.Find(id);
            if (wva == null)
            {
                return HttpNotFound();
            }
            return View(wva);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // POST: Wvas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Id")] Wva wva)
        {
            try { 
            if (ModelState.IsValid)
            {
                db.Entry(wva).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wva);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // GET: Wvas/Delete/5
        public ActionResult Delete(int? id)
        {
            try { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wva wva = db.Wva.Find(id);
            if (wva == null)
            {
                return HttpNotFound();
            }
            return View(wva);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // POST: Wvas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
            Wva wva = db.Wva.Find(id);
            db.Wva.Remove(wva);
                List<BadBee.Core.DAL.BadBee> badbee = db.BadBee.Where(q => q.WvaId == id).ToList();
                foreach (var item in badbee)
                {
                    db.BadBee.Remove(item);
                }
           
            db.SaveChanges();

            GlobalVars.DictionaryCache = new Dictionary<Type, object>();
           BadBee.Core.Providers.ListProvider.FillDictionaryCache();

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
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
