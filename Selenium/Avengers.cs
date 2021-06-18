using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Interactions;

namespace Proyecto_TruequesUCR.Selenium
{
    [TestClass]
    public class Avengers
    {
        IWebDriver driver;
        [TestCleanup]
        public void TearDown()
        {
            if (driver != null)
                driver.Quit();
        }


        [TestMethod]
        public void PruebaIngresoChrome()
        {
            ///Arrange
            /// Crea el driver de Chrome
            driver = new ChromeDriver();
            PruebaIngreso();
        }

        private void PruebaIngreso()
        {
            ///Arrange
            /// Pone la pantalla en full screen
            driver.Manage().Window.Maximize();
            ///Act
            /// Se va a la URL de la aplicacion
            driver.Url = "https://localhost:44358/";
            ///Assert
            Assert.AreEqual(driver.FindElement(By.XPath("//h2")).Text, "Productos más recientes");
        }

        [TestMethod]
        public void PruebaLoginChrome()
        {
            ///Arrange
            /// Crea el driver de Chrome
            driver = new ChromeDriver();
            PruebaLogin();
        }

        public void PruebaLogin()
        {
            ///Arrange
            /// Pone la pantalla en full screen
            driver.Manage().Window.Maximize();
            ///Act
            /// Se va a la URL de la aplicacion
            driver.Url = "https://localhost:44358/Usuario/Login";
            driver.FindElement(By.Id("correo")).SendKeys("default@gmail.com");
            driver.FindElement(By.Id("contrasena")).SendKeys("aa1234!!");
            driver.FindElement(By.Id("boton_IS")).Click();
            ///Assert
            Assert.AreEqual(driver.FindElement(By.Id("recomendaciones")).Text, "Recomendaciones"); //Prueba que salga la opción de "Recomendaciones" que solo sale si el usuario hizo login
        }

        [TestMethod]
        public void PruebaIngresoARecomendacionesChrome()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/Recomendaciones/RecomendacionesPorGustos";

            Assert.AreEqual(driver.FindElement(By.Id("searchbar")).GetAttribute("placeholder"), "Buscar por #hashtag, categoria o nombre del ítem...");
        }

        [TestMethod]
        public void PruebaRealizarBusqueda()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/Home/Index";

            string originalWindow = driver.CurrentWindowHandle;
            Assert.AreEqual(driver.WindowHandles.Count, 1);


            driver.FindElement(By.Id("searchbar")).SendKeys("iPhone");
            IWebElement elemento = driver.FindElement(By.Id("searchitems"));
            System.Threading.Thread.Sleep(5000);
            elemento.Click();


            Assert.AreEqual(driver.FindElement(By.Id("migasDePan")).Text, "Resultado de búsqueda");
            Assert.AreEqual(driver.FindElement(By.Id("resultado")).Text, "Resultados de búsqueda");
        }

        [TestMethod]
        public void PruebaEscogerFiltroDirecto()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/InformacionItemBuscado/PruebaBuscarItemsPorCriterio?criterioBusqueda=iPhone";

            string originalWindow = driver.CurrentWindowHandle;
            Assert.AreEqual(driver.WindowHandles.Count, 1);

            IWebElement elemento2 = driver.FindElement(By.Id("trueque_uno_uno"));
            System.Threading.Thread.Sleep(5000);
            elemento2.Click();

            bool isChecked = driver.FindElement(By.Id("trueque_uno_uno")).Selected;

            Assert.AreEqual(driver.FindElement(By.Id("migasDePan")).Text, "Resultado de búsqueda");
            Assert.AreEqual(driver.FindElement(By.Id("resultado")).Text, "Resultados de búsqueda");
            Assert.AreEqual(isChecked, true);

        }

        [TestMethod]
        public void PruebaEscogerFiltroSubasta()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/InformacionItemBuscado/PruebaBuscarItemsPorCriterio?criterioBusqueda=iPhone";

            string originalWindow = driver.CurrentWindowHandle;
            Assert.AreEqual(driver.WindowHandles.Count, 1);

            IWebElement elemento2 = driver.FindElement(By.Id("trueque_subasta"));
            System.Threading.Thread.Sleep(5000);
            elemento2.Click();

            bool isChecked = driver.FindElement(By.Id("trueque_subasta")).Selected;

            Assert.AreEqual(driver.FindElement(By.Id("migasDePan")).Text, "Resultado de búsqueda");
            Assert.AreEqual(driver.FindElement(By.Id("resultado")).Text, "Resultados de búsqueda");
            Assert.AreEqual(isChecked, true);

        }

        [TestMethod]
        public void PruebaEscogerFiltroCircular()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/InformacionItemBuscado/PruebaBuscarItemsPorCriterio?criterioBusqueda=iPhone";

            string originalWindow = driver.CurrentWindowHandle;
            Assert.AreEqual(driver.WindowHandles.Count, 1);

            IWebElement elemento2 = driver.FindElement(By.Id("trueque_circular"));
            System.Threading.Thread.Sleep(5000);
            elemento2.Click();

            bool isChecked = driver.FindElement(By.Id("trueque_circular")).Selected;

            Assert.AreEqual(driver.FindElement(By.Id("migasDePan")).Text, "Resultado de búsqueda");
            Assert.AreEqual(driver.FindElement(By.Id("resultado")).Text, "Resultados de búsqueda");
            Assert.AreEqual(isChecked, true);

        }

        [TestMethod]
        public void PruebaLimpiarFiltros()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            driver.Url = "https://localhost:44358/InformacionItemBuscado/PruebaBuscarItemsPorCriterio?criterioBusqueda=iPhone";

            string originalWindow = driver.CurrentWindowHandle;
            Assert.AreEqual(driver.WindowHandles.Count, 1);

            IWebElement elemento2 = driver.FindElement(By.Id("trueque_nuevo"));
            System.Threading.Thread.Sleep(5000);
            elemento2.Click();

            IWebElement elemento3 = driver.FindElement(By.Id("trueque_casiNuevo"));
            System.Threading.Thread.Sleep(5000);
            elemento3.Click();

            bool isChecked2 = driver.FindElement(By.Id("trueque_nuevo")).Selected;
            bool isChecked3 = driver.FindElement(By.Id("trueque_casiNuevo")).Selected;

            Assert.AreEqual(isChecked2, true);
            Assert.AreEqual(isChecked3, true);

            IWebElement elemento4 = driver.FindElement(By.Id("limpiarFiltrosBtn"));
            System.Threading.Thread.Sleep(5000);
            elemento4.Click();

            isChecked2 = driver.FindElement(By.Id("trueque_nuevo")).Selected;
            isChecked3 = driver.FindElement(By.Id("trueque_casiNuevo")).Selected;

            Assert.AreEqual(isChecked2, false);
            Assert.AreEqual(isChecked3, false);
        }

        [TestMethod]
        public void PruebaRealizarBusquedaPorTitulo()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Url = "https://localhost:44358/Home/Index";

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3); // Tiempo de espera para que los elementos html se hagan visibles y se pueda interactuar con ellos.
            driver.FindElement(By.Id("searchbar")).Click(); // Click en la barra de búsqueda.
            driver.FindElement(By.Id("searchbar")).SendKeys("Gema de la Realidad"); // Escribe texto en la barra de búsqueda.
            driver.FindElement(By.Id("searchitems")).Click(); // Click a la lupita para realizar la búsqueda.

            // El sistema despliega el resultado de búsqueda.

            var titulo = driver.FindElement(By.ClassName("card-title")).GetAttribute("innerHTML"); // Extrae el título del item encontrado.
            Assert.AreEqual(titulo, "Gema de la Realidad");
        }

        [TestMethod]
        public void PruebaRealizarBusquedaYClickearPopUpYVerificarTitulo()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Url = "https://localhost:44358/Home/Index";

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3); // Tiempo de espera para que los elementos html se hagan visibles y se pueda interactuar con ellos.
            driver.FindElement(By.Id("searchbar")).Click(); // Click en la barra de búsqueda.
            driver.FindElement(By.Id("searchbar")).SendKeys("Libro OS"); // Escribe texto en la barra de búsqueda.
            driver.FindElement(By.Id("searchitems")).Click(); // Click a la lupita para realizar la búsqueda.

            // El sistema despliega el resultado de búsqueda.

            driver.FindElement(By.Name("Monopoly2")).Click(); // Click en ver producto para recomendación.
            var modal = driver.FindElement(By.Id("Monopoly2")); // Encuentra el modal.
            var titulo = modal.FindElement(By.ClassName("card-text")).GetAttribute("innerHTML"); // Extrae el título del item encontrado de la ventana emergente.

            Assert.AreEqual(titulo, " - Título:  Monopoly ");
        }
    }
}
