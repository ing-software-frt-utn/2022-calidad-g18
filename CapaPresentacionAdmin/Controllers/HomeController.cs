//using CapaPresentacionAdmin.Models;
using CapaPresentacionAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionAdmin.Controllers
{
    public class HomeController : Controller
    {
        private FabricaCalzadosDB db = new FabricaCalzadosDB();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VistaDashboard()
        {
            int totalModelos = db.Modelo.Count();
            int totalColores = db.Color.Count();
            int totalEmpleados = db.Empleado.Count();

            var data = new
            {
                TotalModelos = totalModelos,
                TotalColores = totalColores,
                TotalEmpleados = totalEmpleados
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}