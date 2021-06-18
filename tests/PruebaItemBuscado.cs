using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proyecto_TruequesUCR;
using Proyecto_TruequesUCR.Controllers;
using System.Web.UI.WebControls;
using Proyecto_TruequesUCR.Models;
using System.Data;
using System.Data.Entity;
using System.Web;
using Moq;

namespace Proyecto_TruequesUCR.Tests.Controllers
{
    [TestClass]
    public class PruebaItemBuscado
    {
        private ProyectoEntities db = new ProyectoEntities();
        [TestMethod]
        public void TestInformacionItemNull()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 1, 1, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestInformacionItem01()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 1, 12, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestInformacionItem02()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 12, 1, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestInformacionItem03()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 1, 0, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestInformacionItem04()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 0, 1, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestInformacionItem05()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult resultado = controller.DesplegarResultadosDeBusqueda("", 0, 0, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestOrdenamiento01()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.ordenarResultadoDeBusqueda(-1);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestOrdenamiento02()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.ordenarResultadoDeBusqueda(50);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestOrdenamiento03()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.ordenarResultadoDeBusqueda(null);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestLimpiarFiltro()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.limpiarFiltros();
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorTueque01()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorTrueque(null, null, null);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorTueque02()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorTrueque(null, false, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorTueque03()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorTrueque(false, false, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorTueque04()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorTrueque(true, true, true);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorTueque05()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorTrueque(true, false, false);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorCalificacion01()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorCalificacion(-1);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorCalificacion02()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorCalificacion(10);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestFiltrarPorCalificacion03()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorCalificacion(4);
            Assert.IsNotNull(resultado);
        }

        /*
         * Inicio de actividad supervisada #5 30/06/2020
         * Participantes Alejandro Abarca y Dennis Solano.
         * Historia trabajada: PIG01-151.
         * Controlador a probar(pruebas unitarias y de integración): PruebaBuscarItemPorCriterio.
         * Pruebas de UI (Selenium): PruebaBuscarItemPorCriterio.
        */

        //En validacion tecnica se determino que la siguiente prueba de unidad requiere de una refactorizacion grande de codigo
        [TestMethod]
        public void TestResultadosBusquedaPorCategoria()
        {
            var mockDb = new Mock<ProyectoEntities>();

            string idCategoria = "Iphone";
            Categoria categoria = new Categoria()
            {
                Nombre_Categoria = idCategoria,
                Padre = "Celular"
            };

            string idComerciante = "default@gmail.com";
            Comerciante comerciante = new Comerciante()
            {
                Email = idComerciante,
                Nombre = "Ronald",
                Apellido = "Avenger",
                Activo = 1
            };

            int idTrueque = 1;
            Trueques trueque1 = new Trueques()
            {
                IdTrueque = idTrueque,
                Descripcion = "Trueque directo de Iphone",
                Estado = "Nuevo",
                FechaInicio = Convert.ToDateTime("2020-05-30 00:00:00.000"),
                FechaFinal = Convert.ToDateTime("2020-06-15 00:00:00.000"),
                Tipo = "Directo",
                FechaCreacion = Convert.ToDateTime("2020-05-25 00:00:00.000")
            };

            Directo truequeDirecto = new Directo()
            {
                IdTrueque = 1,
            };

            int idItem = 2;

            Item item = new Item()
            {
                Id_Item = idItem,
                Email_Comerciante = "default@gmail.com",
                Descripcion = "Iphone 7 nuevo",
                Titulo = "Iphone 7",
                Fecha = Convert.ToDateTime("2020-05-30 00:00:00.000"),
                Tipo_Visibilidad = "Publico",
                Nombre_Categoria_Item = "Iphone",
                IdTrueque = 1,
                Eliminado = false
            };
            mockDb.Setup(m => m.Comerciante.Find("default@gmail.com")).Returns(comerciante);
            mockDb.Setup(m => m.Item.Find(idItem, "default@gmail.com")).Returns(item);
            mockDb.Setup(m => m.Trueques.Find(idTrueque)).Returns(trueque1);
            mockDb.Setup(m => m.Categoria.Find(idCategoria)).Returns(categoria);
        }

        [TestMethod]
        public void TestBusquedaNotNull()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ViewResult result = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true) as ViewResult;
            Assert.IsNotNull(result.Model);
        }


        [TestMethod]
        public void TestFiltrarResultadoBusquedaPorUbicacionNotNull()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorUbicacion(null, null, null);
            Assert.IsNotNull(resultado);
        }


        [TestMethod]
        public void TestFiltrarResultadoBusquedaPorUbicacionPaisNoExistente()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorUbicacion("China", "Hubei", "Wuhan");
            Assert.IsNotNull(resultado);
        }


        [TestMethod]
        public void TestFiltrarResultadoBusquedaPorUbicacionPaisExistente()
        {
            InformacionItemBuscadoController controller = new InformacionItemBuscadoController();
            ActionResult var = controller.DesplegarResultadosDeBusqueda("iphone", 1, 6, true);
            ActionResult resultado = controller.filtrarResultadoDeBusquedaPorUbicacion("Costa Rica", "San José", "Montes De Oca");
            Assert.IsNotNull(resultado);
        }
    }

}
