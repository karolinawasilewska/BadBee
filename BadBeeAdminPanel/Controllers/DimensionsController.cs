using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BadBee.Core.DAL;

namespace BadBeeAdminPanel.Controllers
{
    public class DimensionsController : Controller
    {
        private BadBeeEntities db = new BadBeeEntities();

        // GET: Dimensions
        public ActionResult Index()
        {
            var dimension = db.Dimension.Include(d => d.Height).Include(d => d.Thickness).Include(d => d.Width);
            return View(dimension.ToList());
        }

        // GET: Dimensions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dimension dimension = db.Dimension.Find(id);
            if (dimension == null)
            {
                return HttpNotFound();
            }
            return View(dimension);
        }

        // GET: Dimensions/Create
        public ActionResult Create()
        {
            ViewBag.HeightId = new SelectList(db.Height, "HeightId", "HeightId");
            ViewBag.ThicknessId = new SelectList(db.Thickness, "ThicknessId", "ThicknessId");
            ViewBag.WidthId = new SelectList(db.Width, "WidthId", "WidthId");
            return View();
        }

        // POST: Dimensions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DimensionId,WidthId,HeightId,ThicknessId")] Dimension dimension)
        {
            if (ModelState.IsValid)
            {
                db.Dimension.Add(dimension);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HeightId = new SelectList(db.Height, "HeightId", "HeightId", dimension.HeightId);
            ViewBag.ThicknessId = new SelectList(db.Thickness, "ThicknessId", "ThicknessId", dimension.ThicknessId);
            ViewBag.WidthId = new SelectList(db.Width, "WidthId", "WidthId", dimension.WidthId);
            return View(dimension);
        }

        // GET: Dimensions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dimension dimension = db.Dimension.Find(id);
            if (dimension == null)
            {
                return HttpNotFound();
            }
            ViewBag.HeightId = new SelectList(db.Height, "HeightId", "HeightId", dimension.HeightId);
            ViewBag.ThicknessId = new SelectList(db.Thickness, "ThicknessId", "ThicknessId", dimension.ThicknessId);
            ViewBag.WidthId = new SelectList(db.Width, "WidthId", "WidthId", dimension.WidthId);
            return View(dimension);
        }

        // POST: Dimensions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DimensionId,WidthId,HeightId,ThicknessId")] Dimension dimension)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dimension).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HeightId = new SelectList(db.Height, "HeightId", "HeightId", dimension.HeightId);
            ViewBag.ThicknessId = new SelectList(db.Thickness, "ThicknessId", "ThicknessId", dimension.ThicknessId);
            ViewBag.WidthId = new SelectList(db.Width, "WidthId", "WidthId", dimension.WidthId);
            return View(dimension);
        }

        // GET: Dimensions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dimension dimension = db.Dimension.Find(id);
            if (dimension == null)
            {
                return HttpNotFound();
            }
            return View(dimension);
        }

        // POST: Dimensions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dimension dimension = db.Dimension.Find(id);
            db.Dimension.Remove(dimension);
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
