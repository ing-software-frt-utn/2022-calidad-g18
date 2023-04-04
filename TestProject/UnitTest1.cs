using CapaPresentacionAdmin.Controllers;
using CapaPresentacionAdmin.Models;
using Moq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace LoginTests
{
    [TestClass]
    public class LoginControllerTests
    {
        private LoginController loginController;

        [TestInitialize]
        public void Setup()
        {
            loginController = new LoginController();
        }

        [TestMethod]
        public void LoginAdmin()
        {
            // Arrange
            var empleado = new Empleado
            {
                Correo = "tobiasarnedo@gmail.com",
                Clave = "Cal123"
            };
            var expected = true;

            // Act
            var result = loginController.Login(empleado.Correo, empleado.Clave);

            // Assert
            Assert.AreEqual<bool>(expected, result);
        }
    }
}
