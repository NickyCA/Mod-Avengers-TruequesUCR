using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proyecto_TruequesUCR;
using Proyecto_TruequesUCR.Controllers;

namespace Proyecto_TruequesUCR.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        /*Historia ID: AVG_SRM4
        * Yo como Usuario sin registrarse   quiero al abrir la aplicación, ver los productos más recientemente agregados   
        * para valorar si me gustaria loguearme/registrarme y también poder ver qué tipo de productos me ofrece la aplicación.
        * Tareas tecnicas: Pruebas de integración para revisar el despliegue de los items más recientes. Camino feliz / camino triste. 
        * Pruebas de unidad para revisar el estado de la página.
         Participantes: Nicole Castillo, Fabián Jiménez
        */

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index(1,12) as ViewResult; // OJO CON ESTO PARA EL FUTURO.

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void IndexPruebaNegativos() 
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Index(-1, -89) as ViewResult; 

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexPruebaNumerosGrandes()
        {
            HomeController controller = new HomeController();
            ViewResult result = controller.Index(1000, 1000) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
