//using CapaPresentacionAdmin.Models;
using CapaPresentacionAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaPresentacionAdmin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            using (var context = new FabricaCalzadosDB())
            {
                Empleado empleado = context.Empleado.FirstOrDefault(u => u.Correo == correo && u.Clave == clave);

                if (empleado != null)
                {
                    // Crear sesión para el usuario
                    Session["Empleado"] = empleado;

                    // Agregar cookie de autenticación
                    HttpCookie authCookie = FormsAuthentication.GetAuthCookie(empleado.Correo, false);
                    Response.Cookies.Add(authCookie);

                    if (empleado.TipoEmpleado == "Administrador")
                    {
                        return RedirectToAction("Index", "Home");
                    }                        
                    else if (empleado.TipoEmpleado == "SupervisorLinea")
                    {
                        return RedirectToAction("Index", "OrdenesProduccion");
                    }
                    else if (empleado.TipoEmpleado == "SupervisorCalidad")
                    {
                        return RedirectToAction("Index", "Calidad");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }   
                }
                else
                {
                    ViewBag.Mensaje = "Correo o clave inválidos";
                    return View();
                }
            }

        }

        public bool Login(string correo, string clave)
        {
            // Lógica de autenticación
            if (correo == "tobiasarnedo@gmail.com" && clave == "Cal123")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Registrar()
        {
            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session["Usuario"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}