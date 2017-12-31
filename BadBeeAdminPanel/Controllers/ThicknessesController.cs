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
using BadBee.Core.Providers;

namespace BadBeeAdminPanel.Controllers
{
    [Authorize]
    public class ThicknessesController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ThicknessesController));

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

        // GET: Thicknesses
        public ActionResult Index()
        {
            return View(db.Thickness.OrderBy(q=>q.Thickness1).ToList());
        }

        // GET: Thicknesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thickness thicknesses = db.Thickness.Find(id);
            if (thicknesses == null)
            {
                return HttpNotFound();
            }
            return View(thicknesses);
        }

        // POST: Thicknesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Thickness thicknesses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thicknesses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thicknesses);
        }

        // GET: Thicknesses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thickness thicknesses = db.Thickness.Find(id);
            if (thicknesses == null)
            {
                return HttpNotFound();
            }
            return View(thicknesses);
        }

        // POST: Thicknesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thickness thicknesses = db.Thickness.Find(id);
            db.Thickness.Remove(thicknesses);
            List<Item> items = db.Item.Where(q => q.BadBee.Dimension.Thickness.ThicknessId == thicknesses.ThicknessId).ToList();

            foreach (var item in items)
            {
                db.Item.Remove(item);
            }
            db.SaveChanges();

            GlobalVars.DictionaryCache = new Dictionary<Type, object>();
            ListProvider.FillDictionaryCache();

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
