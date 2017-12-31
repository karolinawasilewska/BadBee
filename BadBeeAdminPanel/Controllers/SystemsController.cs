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
using System.Threading;
using System.Globalization;
using BadBee.Core.Models;

namespace BadBeeAdminPanel.Controllers
{
    [Authorize]
    public class SystemsController : Controller
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(SystemsController));

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

        // GET: Systems
        public ActionResult Index()
        {
            return View(db.Systems.OrderBy(q=>q.Abbreviation).ToList());
        }
       
        // GET: Systems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Systems systems = db.Systems.Find(id);
            if (systems == null)
            {
                return HttpNotFound();
            }
            return View(systems);
        }

        // POST: Systems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Systems systems)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systems).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systems);
        }

        // GET: Systems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Systems systems = db.Systems.Find(id);
            if (systems == null)
            {
                return HttpNotFound();
            }
            return View(systems);
        }

        // POST: Systems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Systems systems = db.Systems.Find(id);
            db.Systems.Remove(systems);
            List<Item> items = db.Item.Where(q => q.BadBee.SystemId == systems.SystemId).ToList();

            foreach (var item in items)
            {
                db.Item.Remove(item);
            }
            db.SaveChanges();

            GlobalVars.DictionaryCache = new Dictionary<Type, object>();
            BadBee.Core.Providers.ListProvider.FillDictionaryCache();

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
