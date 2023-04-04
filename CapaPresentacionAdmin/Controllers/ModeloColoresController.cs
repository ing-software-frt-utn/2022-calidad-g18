using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapaPresentacionAdmin.Models;

namespace CapaPresentacionAdmin.Controllers
{
    public class ModeloColoresController : Controller
    {
        private FabricaCalzadosDB db = new FabricaCalzadosDB();

        // GET: ModeloColores
        public ActionResult Index()
        {
            var modeloColor = db.ModeloColor.Include(m => m.Color).Include(m => m.Modelo);
            return View(modeloColor.ToList());
        }

        // GET: ModeloColores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloColor modeloColor = db.ModeloColor.Find(id);
            if (modeloColor == null)
            {
                return HttpNotFound();
            }
            return View(modeloColor);
        }

        // GET: ModeloColores/Create
        public ActionResult Create()
        {
            ViewBag.Codigo = new SelectList(db.Color, "Codigo", "Descripcion");
            ViewBag.IdModelo = new SelectList(db.Modelo, "IdModelo", "SKU");
            return View();
        }

        // POST: ModeloColores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdModeloColor,IdModelo,Codigo,FechaRegistro")] ModeloColor modeloColor)
        {
            if (ModelState.IsValid)
            {
                modeloColor.FechaRegistro = DateTime.Now;
                db.ModeloColor.Add(modeloColor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Codigo = new SelectList(db.Color, "Codigo", "Descripcion", modeloColor.Codigo);
            ViewBag.IdModelo = new SelectList(db.Modelo, "IdModelo", "SKU", modeloColor.IdModelo);
            return View(modeloColor);
        }

        // GET: ModeloColores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloColor modeloColor = db.ModeloColor.Find(id);
            if (modeloColor == null)
            {
                return HttpNotFound();
            }
            ViewBag.Codigo = new SelectList(db.Color, "Codigo", "Descripcion", modeloColor.Codigo);
            ViewBag.IdModelo = new SelectList(db.Modelo, "IdModelo", "SKU", modeloColor.IdModelo);
            return View(modeloColor);
        }

        // POST: ModeloColores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdModeloColor,IdModelo,Codigo,FechaRegistro")] ModeloColor modeloColor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modeloColor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Codigo = new SelectList(db.Color, "Codigo", "Descripcion", modeloColor.Codigo);
            ViewBag.IdModelo = new SelectList(db.Modelo, "IdModelo", "SKU", modeloColor.IdModelo);
            return View(modeloColor);
        }

        // GET: ModeloColores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloColor modeloColor = db.ModeloColor.Find(id);
            if (modeloColor == null)
            {
                return HttpNotFound();
            }
            return View(modeloColor);
        }

        // POST: ModeloColores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModeloColor modeloColor = db.ModeloColor.Find(id);
            db.ModeloColor.Remove(modeloColor);
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
