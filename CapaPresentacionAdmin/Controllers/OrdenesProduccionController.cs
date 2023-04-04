using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CapaPresentacionAdmin.Models;

namespace CapaPresentacionAdmin.Controllers
{
    public class OrdenesProduccionController : Controller
    {
        private FabricaCalzadosDB db = new FabricaCalzadosDB();

        // GET: OrdenProduccions
        public ActionResult Index()
        {
            // Obtener el empleado logueado
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);

            // Verificar si el supervisor está trabajando actualmente en una orden de producción
            var trabajandoActualmente = db.OrdenProduccion.Any(o => o.SupervisorLinea == empleado.DNI && o.Estado != "Finalizada");

            // Crear un ViewBag para pasar la variable trabajandoActualmente a la vista
            ViewBag.TrabajandoActualmente = trabajandoActualmente;

            // Obtener la orden de producción en la que el supervisor está trabajando actualmente, si hay alguna
            var ordenProduccion = db.OrdenProduccion
                .Include(o => o.Color)
                .Include(o => o.Empleado)
                .Include(o => o.Empleado1)
                .Include(o => o.JornadaLaboral)
                .Include(o => o.Linea)
                .Include(o => o.Modelo)
                .FirstOrDefault(o => o.SupervisorLinea == empleado.DNI && o.Estado != "Finalizada");

            // Crear una lista con la orden de producción obtenida, o una lista vacía si no hay ninguna
            var ordenesProduccion = ordenProduccion != null ? new List<OrdenProduccion> { ordenProduccion } : new List<OrdenProduccion>();

            return View(ordenesProduccion);
        }

        // GET: OrdenProduccions/Create
        public ActionResult Create()
        {
            // Obtener el empleado logueado
            var empleado = (Empleado)Session["Empleado"];

            // Obtener líneas sin una OP en curso
            var lineasDisponibles = db.Linea.Where(l => !db.OrdenProduccion.Any(op => op.NumeroLinea == l.NumeroLinea && op.Estado == "Iniciada"));

            ViewBag.NumeroLinea = new SelectList(lineasDisponibles, "NumeroLinea", "NumeroLinea");
            ViewBag.Modelos = new SelectList(db.Modelo, "IdModelo", "Denominacion");
            ViewBag.Colores = Enumerable.Empty<SelectListItem>();
            return View();
        }

        [HttpGet]
        public JsonResult GetColoresByModelo(int idModelo)
        {
            var colores = db.ModeloColor
                            .Where(mc => mc.IdModelo == idModelo)
                            .Select(mc => new { mc.Color.Codigo, mc.Color.Descripcion })
                            .ToList();

            return Json(colores, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "NumeroOP,NumeroLinea,IdModelo,Codigo,Estado,IdJornadaLaboral,SupervisorLinea,SupervisorCalidad,FechaRegistro")] OrdenProduccion ordenProduccion)
        {
            if (ModelState.IsValid)
            {
                ordenProduccion.Estado = "Iniciada";
                ordenProduccion.FechaRegistro = DateTime.Now;

                // Obtener supervisor de línea
                string correoSupervisorLinea = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                Empleado supervisorLinea = db.Empleado.FirstOrDefault(e => e.Correo == correoSupervisorLinea);

                if (supervisorLinea != null)
                {
                    if (db.OrdenProduccion.Any(op => op.NumeroOP == ordenProduccion.NumeroOP))
                    {
                        // Si existe una OP con el mismo número, mostrar un mensaje de error
                        ModelState.AddModelError("", "Ya existe una orden de producción con el mismo número");

                        // Actualizar las opciones de selección
                        ViewBag.NumeroLinea = new SelectList(db.Linea.Where(l => l.OrdenProduccion.All(op => op.Estado != "Iniciada")), "NumeroLinea", "NumeroLinea");
                        ViewBag.Modelos = new SelectList(db.Modelo, "IdModelo", "Denominacion");
                        ViewBag.Colores = Enumerable.Empty<SelectListItem>();
                        return View(ordenProduccion);
                    }

                    // Comprobar si la línea seleccionada está disponible
                    if (db.OrdenProduccion.Any(op => op.NumeroLinea == ordenProduccion.NumeroLinea && op.Estado == "Iniciada"))
                    {
                        // Si la línea está en uso, mostrar un mensaje de error
                        ModelState.AddModelError("", "La línea seleccionada ya está en uso");

                        // Actualizar las opciones de selección
                        ViewBag.NumeroLinea = new SelectList(db.Linea.Where(l => l.OrdenProduccion.All(op => op.Estado != "Iniciada")), "NumeroLinea", "NumeroLinea");
                        ViewBag.Modelos = new SelectList(db.Modelo, "IdModelo", "Denominacion");
                        ViewBag.Colores = Enumerable.Empty<SelectListItem>();
                        return View(ordenProduccion);
                    }

                    if (db.OrdenProduccion.Any(op => op.SupervisorLinea == supervisorLinea.DNI && op.Estado != "Finalizada"))
                    {
                        // Si el supervisor tiene una OP no finalizada, mostrar un mensaje de error
                        ModelState.AddModelError("", "El supervisor de línea tiene una orden de producción no finalizada");

                        // Actualizar las opciones de selección
                        ViewBag.NumeroLinea = new SelectList(db.Linea.Where(l => l.OrdenProduccion.All(op => op.Estado != "Iniciada")), "NumeroLinea", "NumeroLinea");
                        ViewBag.Modelos = new SelectList(db.Modelo, "IdModelo", "Denominacion");
                        ViewBag.Colores = Enumerable.Empty<SelectListItem>();
                        return View(ordenProduccion);
                    }

                    ordenProduccion.SupervisorLinea = supervisorLinea.DNI;
                    db.OrdenProduccion.Add(ordenProduccion);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error al obtener el supervisor de línea");
                }
            }

            ViewBag.NumeroLinea = new SelectList(db.Linea.Where(l => l.OrdenProduccion.All(op => op.Estado != "Iniciada")), "NumeroLinea", "NumeroLinea");
            ViewBag.IdModelo = new SelectList(db.Modelo, "IdModelo", "Descripcion", ordenProduccion.IdModelo);
            ViewBag.Codigo = new SelectList(db.Color, "Codigo", "Descripcion", ordenProduccion.Codigo);
            return View(ordenProduccion);
        }

        [HttpPost]
        public ActionResult Pausar(int id)
        {
            OrdenProduccion op = db.OrdenProduccion.Find(id);
            if (op != null)
            {
                op.Estado = "Pausada";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Reanudar(int id)
        {
            OrdenProduccion op = db.OrdenProduccion.Find(id);
            if (op != null)
            {
                op.Estado = "Iniciada";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Finalizar(int id)
        {
            OrdenProduccion op = db.OrdenProduccion.Find(id);
            if (op != null)
            {
                op.Estado = "Finalizada";
                db.SaveChanges();
            }
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
