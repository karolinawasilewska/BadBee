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
using log4net;
using System.Globalization;
using BadBee.Core.Models;
using BadBee.Core.Providers;

namespace BadBeeAdminPanel.Controllers
{
    [Authorize]
    public class BadBeeNumbersController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(BadBeeNumbersController));

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

        // GET: BadBeeNumbers
        public ActionResult Index()
        {
            return View(db.BadBee.OrderBy(q=>q.BadBeeNo).ToList());
        }
        
        // GET: BadBeeNumbers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           BadBee.Core.DAL.BadBee BadBeeNumbers = db.BadBee.Find(id);
            if (BadBeeNumbers == null)
            {
                return HttpNotFound();
            }
            return View(BadBeeNumbers);
        }

        // POST: BadBeeNumbers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BadBeeNumber,BadBeeNumberId")] BadBee.Core.DAL.BadBee BadBeeNumbers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(BadBeeNumbers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(BadBeeNumbers);
        }

        // GET: BadBeeNumbers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BadBee.Core.DAL.BadBee BadBeeNumbers = db.BadBee.Find(id);
            if (BadBeeNumbers == null)
            {
                return HttpNotFound();
            }
            return View(BadBeeNumbers);
        }

        // POST: BadBeeNumbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BadBee.Core.DAL.BadBee BadBeeNumbers = db.BadBee.Find(id);
            db.BadBee.Remove(BadBeeNumbers);
            List<Item> items = db.Item.Where(q => q.BadBee.BadBeeNo == BadBeeNumbers.BadBeeNo).ToList();

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
