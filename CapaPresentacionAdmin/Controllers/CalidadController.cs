using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CapaPresentacionAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient; 

namespace CapaPresentacionAdmin.Controllers
{

    public class CalidadController : Controller
    {

        private FabricaCalzadosDB db = new FabricaCalzadosDB();

        public ActionResult Index()
        {
            // Verificar si el empleado está asociado a una OP
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);
            var opAsociada = db.OrdenProduccion.Any(op => op.SupervisorCalidad == empleado.DNI);

            // Pasar el resultado de la verificación a la vista
            ViewBag.OpAsociada = opAsociada;
            return View();
        }

        public ActionResult Inspeccionar()
        {
            // Obtener el empleado logueado
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);

            var ordenProduccion = db.OrdenProduccion.FirstOrDefault(op => op.SupervisorCalidad == empleado.DNI);

            if (ordenProduccion == null || ordenProduccion.SupervisorCalidad == null)
            {
                TempData["ErrorMessage"] = "Primero debe estar asociado a una OP.";
                return RedirectToAction("Index");
            }

            return View("Inspeccionar", ordenProduccion);

        }

        [HttpPost]
        public ActionResult Inspeccionar(TimeSpan hora, int totalPrimera, List<Defecto> defectos)
        {
            // Obtener el empleado logueado
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);

            var ordenProduccion = db.OrdenProduccion.FirstOrDefault(op => op.SupervisorCalidad == empleado.DNI);

            var jornadaLaboral = db.JornadaLaboral.FirstOrDefault(j => j.IdJornadaLaboral == ordenProduccion.IdJornadaLaboral);

            if (jornadaLaboral == null)
            {
                TempData["ErrorMessage"] = "No se pudo encontrar la jornada laboral correspondiente.";
                return RedirectToAction("Index");
            }

            jornadaLaboral.HoraRegistro = hora;
            jornadaLaboral.TotalPrimera = totalPrimera;
            //jornadaLaboral.Defecto| = defectos;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Los datos de la inspección se han guardado exitosamente.";

            return RedirectToAction("Index");
        }

        public ActionResult AsociarOP()
        {
            // Obtener la lista de órdenes de producción sin SupervisorCalidad
            var ordenesProduccion = db.OrdenProduccion.Where(op => op.SupervisorCalidad == null).ToList();

            ViewBag.OrdenesProduccion = ordenesProduccion;

            return View(); 
        }

        private int ObtenerIdTurno(DateTime fecha)
        {
            TimeSpan hora = fecha.TimeOfDay;

            if (hora >= TimeSpan.FromHours(6) && hora < TimeSpan.FromHours(14))
            {
                // Turno mañana
                return 1;
            }
            else if (hora >= TimeSpan.FromHours(14) && hora < TimeSpan.FromHours(22))
            {
                // Turno tarde
                return 2;
            }
            else
            {
                // Turno noche
                return 3;
            }
        }

        [HttpPost]
        public ActionResult AsociarOP(int numeroOp)
        {
            // Obtener el empleado logueado
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);

            // Buscar la orden de producción seleccionada
            var ordenProduccion = db.OrdenProduccion.FirstOrDefault(op => op.NumeroOP == numeroOp);

            if (ordenProduccion != null)
            {
                // Asociar el empleado autenticado como SupervisorCalidad
                ordenProduccion.SupervisorCalidad = empleado.DNI;

                // Obtener el turno actual
                int idTurno = ObtenerIdTurno(DateTime.Now);

                // Crear una nueva jornada laboral con el turno actual y la hora de inicio actual
                var jornadaLaboral = new JornadaLaboral
                {
                    IdTurno = idTurno,
                    HoraInicio = DateTime.Now
                };
                db.JornadaLaboral.Add(jornadaLaboral);
                db.SaveChanges();

                // Asociar la nueva jornada laboral a la orden de producción
                ordenProduccion.IdJornadaLaboral = jornadaLaboral.IdJornadaLaboral;

                db.SaveChanges();
                TempData["SuccessMessage"] = "Se ha asociado a la orden de producción con éxito.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo asociar a la orden de producción seleccionada.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AbandonarOP()
        {
            // Obtener el empleado logueado
            var correoEmpleado = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var empleado = db.Empleado.FirstOrDefault(e => e.Correo == correoEmpleado);
            var ordenProduccion = db.OrdenProduccion.FirstOrDefault(op => op.SupervisorCalidad == empleado.DNI);

            if (ordenProduccion != null)
            {
                ordenProduccion.SupervisorCalidad = null;
                db.SaveChanges();
                return Json(new { success = true, message = "La orden de producción ha sido abandonada con éxito." });
            }
            else
            {
                return Json(new { success = false, message = "No se encontró una orden de producción para abandonar." });
            }
        }

        public JsonResult HorasTurnoActual()
        {
            DateTime fechaActual = DateTime.Now;
            int idTurno = ObtenerIdTurno(fechaActual);
            List<string> horasTurno = new List<string>();

            if (idTurno == 1)
            {
                for (int i = 6; i < 14; i++)
                {
                    horasTurno.Add($"{i.ToString("00")}:00");
                }
            }
            else if (idTurno == 2)
            {
                for (int i = 14; i < 22; i++)
                {
                    horasTurno.Add($"{i.ToString("00")}:00");
                }
            }
            else if (idTurno == 3)
            {
                for (int i = 22; i < 24; i++)
                {
                    horasTurno.Add($"{i.ToString("00")}:00");
                }
                for (int i = 0; i < 6; i++)
                {
                    horasTurno.Add($"{i.ToString("00")}:00");
                }
            }

            return Json(horasTurno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObtenerDefectos(string tipo)
        {
            var defectos = db.Defecto
                .Where(d => d.TipoDefecto == tipo)
                .Select(d => d.Descripcion)
                .ToList();

            return Json(defectos, JsonRequestBehavior.AllowGet);
        }

        
    }

}