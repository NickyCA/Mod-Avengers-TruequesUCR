using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Proyecto_TruequesUCR.Models;
using PagedList;
using System.Web.UI;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using Microsoft.Ajax.Utilities;
using System.Security.Cryptography;

namespace Proyecto_TruequesUCR.Controllers
{
    public class InformacionItemBuscadoController : Controller
    {
        private ProyectoEntities db;
        private ObtenerInfoItems obtenerInfoItems = new ObtenerInfoItems();
        public InformacionItemBuscadoController()
        {
            this.db = new ProyectoEntities();
        }

        public InformacionItemBuscadoController(ProyectoEntities db)
        {
            this.db = db;
        }

        // En este médodo se entra cuando: se hace una nueva búsqueda o se cambia de página.
        public ActionResult DesplegarResultadosDeBusqueda(String criterioBusqueda, int? numeroPagina, int? itemsPorPagina, bool? nuevaBusqueda)
        {
            var resultadosBusqueda = new List<InformacionItemBuscado>();
            int itemsPagina = itemsPorPagina ?? 6;
            TempData["itemsPorPagina"] = itemsPagina;

            int numPagina = numeroPagina ?? 1;
            if (numPagina < 1)
            {
                numPagina = 1;
            }
            if (itemsPagina == 0)
            {
                itemsPagina = 6;
            }

            TempData["numeroPagina"] = numPagina;

            if (!criterioBusqueda.IsNullOrWhiteSpace())
            {
                if (nuevaBusqueda == true || nuevaBusqueda == null)
                {
                    this.limpiarCoockies();
                    TempData["criterioBusqueda"] = criterioBusqueda;
                    resultadosBusqueda = obtenerItems(criterioBusqueda);
                    TempData["ResultadosBusqueda"] = resultadosBusqueda;
                    TempData["estadoActualItems"] = resultadosBusqueda;
                }
                else
                {
                    // Cambio de página.
                    this.obtenerCoockies();
                    resultadosBusqueda = (List<InformacionItemBuscado>)TempData.Peek("estadoActualItems");
                }

                var resultadoBusquedaPaginado = resultadosBusqueda.ToPagedList(numPagina, itemsPagina);
                ViewBag.resultadoBusQuedaPaginado = resultadoBusquedaPaginado;
                ViewBag.busqueda = criterioBusqueda;
                ViewBag.numeroItems = itemsPagina;
                TempData.Keep();

                generarRecomendacionesPorBusquedaActual();
                return View("DesplegarResultadosDeBusqueda", resultadoBusquedaPaginado);
            }

            var resultadoBusquedaPaginado1 = resultadosBusqueda.ToPagedList(numPagina, itemsPagina);
            ViewBag.resultadoBusQuedaPaginado = resultadoBusquedaPaginado1;
            ViewBag.busqueda = criterioBusqueda;
            ViewBag.numeroItems = itemsPagina;
            TempData.Keep();

            return View("DesplegarResultadosDeBusqueda", resultadoBusquedaPaginado1);
        }

        public void generarRecomendacionesPorBusquedaActual()
        {
            var criterio = (string)TempData.Peek("criterioBusqueda");
            var recomendaciones = obtenerListaRecomendacionesPorBusquedaActual(criterio);

            TempData["recomendaciones"] = recomendaciones;
            TempData.Keep();
        }

        private bool esCategoria(String criterio)
        {
            var consulta = (
                   from c in db.Categoria
                   select c.Nombre_Categoria.Replace(" ", "").ToLower()
               ).ToList();
            return consulta.Contains(criterio);
        }

        private bool esEtiqueta(String criterio)
        {
            var consulta = (
                   from e in db.Etiquetas
                   select e.Nombre_Etiqueta.Replace(" ", "").ToLower()
               ).ToList();
            return consulta.Contains(criterio);
        }
        private List<RecomendacionPorBusquedaActual> obtenerListaRecomendacionesPorBusquedaActual(string criterio)
        {
            var recomendacionesCategoria = new List<RecomendacionPorBusquedaActual>();
            String criterioNormalizado = criterio.Replace(" ", "").ToLower();
            bool esCategoria = this.esCategoria(criterioNormalizado);

            if (esCategoria)
            {
                var categoriasAsociadas = obtenerListaDeCategoriasAsociadas(criterioNormalizado);
                recomendacionesCategoria = obtenerRecomendacionesCategorias(categoriasAsociadas);
            }
            else
            {
                recomendacionesCategoria = recomendarTituloHashtag(criterioNormalizado);
            }

            return recomendacionesCategoria;
        }

        private List<RecomendacionPorBusquedaActual> recomendarTituloHashtag(String criterio)
        {
            var recomendacionesCategoria = new List<RecomendacionPorBusquedaActual>();
            List<String> categoriasAsociadas = obtenerListaCategorias(criterio);
            List<String> categoriashermanas = new List<String>();

            foreach (var categoria in categoriasAsociadas)
            {
                categoriashermanas.AddRange(obtenerListaDeCategoriasAsociadas(categoria));
            }
            recomendacionesCategoria = obtenerRecomendacionesCategorias(categoriashermanas.Distinct().ToList());

            return recomendacionesCategoria;

        }

        private List<String> obtenerListaCategorias(string criterio)
        {
            var listaActual = (List<InformacionItemBuscado>)TempData.Peek("estadoActualItems");
            var rand = new Random();
            int size = listaActual.Count();
            List<String> lista = new List<String>();
            int i = 0;
            if (size >= 5)
            {
                for (int ind = 0; ind < 5; ++ind)
                {
                    i = rand.Next(size);
                    lista.Add(listaActual[i].nombreCategoria);
                }
            }
            else
            {
                for (int ind = 0; ind < size; ++ind)
                {
                    i = rand.Next(size);
                    lista.Add(listaActual[i].nombreCategoria);
                }
            }
            return lista;
        }

        private List<String> obtenerListaDeCategoriasAsociadas(String categoria)
        {
            var categoriaPadre =
                (from c in db.Categoria
                 where c.Nombre_Categoria.Replace(" ", "").ToLower().Equals(categoria)
                 select c.Padre).SingleOrDefault();

            var categoriasHermanas =
                (from h in db.Categoria
                 where h.Padre.Equals(categoriaPadre)
                 select h.Nombre_Categoria.Replace(" ", "").ToLower()
                ).ToList();

            categoriasHermanas.Remove(categoria);
            return categoriasHermanas;
        }
        private List<RecomendacionPorBusquedaActual> obtenerRecomendacionesCategorias(List<String> categoriasHermanas)
        {
            List<RecomendacionPorBusquedaActual> recomendaciones = new List<RecomendacionPorBusquedaActual>();

            var items =
            from item in db.Item
            select item;

            foreach (var item in items)
            {
                if (!(bool)(item.Eliminado))
                {
                    if (categoriasHermanas.Contains(item.Categoria.Nombre_Categoria.Replace(" ", "").ToLower()))
                    {
                        recomendaciones.Add(new RecomendacionPorBusquedaActual(item.Titulo, obtenerImagen(item), item.Id_Item, obtenerInfoItems.obtenerPais(item.Email_Comerciante, db), obtenerInfoItems.obtenerProvincia_Estado(item.Email_Comerciante, db),
                            obtenerInfoItems.obtenerCanton_Ciudad(item.Email_Comerciante, db), obtenerInfoItems.obtenerFechaPublicacion(item.Id_Item, item.Email_Comerciante, db), obtenerInfoItems.obtenerCalificacionItem(item.Email_Comerciante, db), obtenerInfoItems.obtenerNombreComerciante(item.Email_Comerciante, db), item.Email_Comerciante));
                    }
                }
            }

            return recomendaciones;
        }

        private String obtenerImagen(Item item)
        {
            byte[] foto = obtenerInfoItems.obtenerDireccionImagen(item.Id_Item, item.Email_Comerciante, db);
            String imagen = "";
            if (foto != null)
            {
                String img64 = Convert.ToBase64String(foto);
                String img64url = string.Format("data:image/" + "jpeg" + ";base64,{0}", img64);
                imagen = img64url;
            }
            return imagen;
        }

        public ActionResult ordenarResultadoDeBusqueda(int? tipoOrdenamiento)
        {
            if (tipoOrdenamiento != 1 && tipoOrdenamiento != 2 && tipoOrdenamiento != 3)
            {
                tipoOrdenamiento = 3;
            }
            ViewBag.tipoOrdenamiento = tipoOrdenamiento;

            var itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            var numeroPagina = (int)TempData.Peek("numeroPagina");

            var resultadosDeBusquedaOrdenados = new List<InformacionItemBuscado>();

            TempData["ordenoPorUbicacion"] = false;
            TempData["ordenoPorFecha"] = false;
            TempData["ordenoPorCalificacion"] = false;

            if (tipoOrdenamiento == 1)
            {
                TempData["ordenoPorCalificacion"] = true;
            }
            else if (tipoOrdenamiento == 2)
            {
                TempData["ordenoPorUbicacion"] = true;
            }
            else if (tipoOrdenamiento == 3)
            {
                TempData["ordenoPorFecha"] = true;
            }
            resultadosDeBusquedaOrdenados = aplicarFiltrosOrdenamiento();
            var resultadosDeBusquedaOrdenadosPaginados = resultadosDeBusquedaOrdenados.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = resultadosDeBusquedaOrdenadosPaginados;

            return View("DesplegarResultadosDeBusqueda", resultadosDeBusquedaOrdenadosPaginados);
        }

        public ActionResult filtrarResultadoDeBusquedaPorUbicacion(string pais, string provincia, string canton)
        {
            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");

            TempData["filtroPorUbicacion"] = true;
            if (pais != null)
            {
                TempData["paisFiltro"] = pais;
                TempData["provinciaFiltro"] = provincia;
                TempData["cantonFiltro"] = canton;
            }
            else if (provincia != null)
            {
                TempData["provinciaFiltro"] = provincia;
                TempData["cantonFiltro"] = canton;
            }
            else if (canton != null)
            {
                TempData["cantonFiltro"] = canton;
            }
            var itemsFiltradosPorUbicacion = aplicarFiltrosOrdenamiento();
            var itemsFiltradosPorUbicacionPaginados = itemsFiltradosPorUbicacion.ToPagedList(numeroPagina, itemsPorPagina);

            ViewBag.TipoOrdenamiento = (int)TempData.Peek("tipoOrdenamiento");

            return View("DesplegarResultadosDeBusqueda", itemsFiltradosPorUbicacionPaginados);
        }

        private List<InformacionItemBuscado> filtrarPorUbicacion(string pais, string provincia, string canton, List<InformacionItemBuscado> items)
        {
            var itemsFiltradosPorUbicacion = new List<InformacionItemBuscado>();

            foreach (InformacionItemBuscado item in items)
            {
                if (!String.IsNullOrEmpty(pais))
                {
                    if (item.pais == pais)
                    {
                        if (!String.IsNullOrEmpty(provincia))
                        {
                            if (item.provincia_Estado == provincia)
                            {
                                if (!String.IsNullOrEmpty(canton))
                                {
                                    if (item.canton_Ciudad == canton)
                                    {
                                        itemsFiltradosPorUbicacion.Add(item);
                                    }
                                }
                                else
                                {
                                    itemsFiltradosPorUbicacion.Add(item);
                                }
                            }
                        }
                        else
                        {
                            itemsFiltradosPorUbicacion.Add(item);
                        }
                    }
                }
                else
                {
                    itemsFiltradosPorUbicacion.Add(item);
                }
            }
            return itemsFiltradosPorUbicacion;
        }

        public ActionResult filtrarResultadoDeBusquedaPorCalificacion(int? cantidadEstrellas)
        {
            /*
             * Si la lista de los items con el estado actual tiene el filtro de calificación aplicado y se quiere aplicar un nuevo filtro, por ejemplo;
             * filtrar por ubicación, entonces hay que recuperar la lista sin el filtrado por estrellas actual (cuando no tenía filtro por calificación aplicado -  lista original)
             * Y aplicarle el nuevo filtro por calificación y el nuevo filtro por ubicación.
             */
            double numero = cantidadEstrellas ?? 5;
            if (numero < 1 || numero > 5)
            {
                numero = 5;
            }
            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");
            var resultadosBusqueda = new List<InformacionItemBuscado>();
            ViewBag.itemsPorPagina = itemsPorPagina;

            TempData["estrellasParaFiltro"] = numero;
            TempData["filtroPorCalificacion"] = true;
            TempData["estrellas"] = cantidadEstrellas; // Para mantener el color de seleccionadas en las estrellitas.

            var itemsFiltradosPorCalificacion = aplicarFiltrosOrdenamiento();
            var itemsPaginados = itemsFiltradosPorCalificacion.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = itemsPaginados;
            ViewBag.TipoOrdenamiento = (int)TempData.Peek("tipoOrdenamiento");
            return View("DesplegarResultadosDeBusqueda", itemsPaginados);
        }


        private List<InformacionItemBuscado> filtrarPorCalificacion(double calificacion, List<InformacionItemBuscado> items)
        {
            var itemsFiltradosPorCalificacion = new List<InformacionItemBuscado>();

            if (calificacion == 5.0)
            {
                foreach (InformacionItemBuscado item in items)
                {
                    if (item.calificacion == calificacion)
                    {
                        itemsFiltradosPorCalificacion.Add(item);
                    }
                }
            }
            else
            {
                foreach (InformacionItemBuscado item in items)
                {
                    if (item.calificacion >= calificacion)
                    {
                        itemsFiltradosPorCalificacion.Add(item);
                    }
                }
            }
            return itemsFiltradosPorCalificacion;
        }


        public ActionResult filtrarResultadoDeBusquedaPorTrueque(bool? subasta, bool? circular, bool? uno)
        {
            /*
             * Si la lista de los items con el estado actual tiene el filtro de calificación aplicado y se quiere aplicar un nuevo filtro, por ejemplo;
             * filtrar por ubicación, entonces hay que recuperar la lista sin el filtrado por estrellas actual (cuando no tenía filtro por calificación aplicado -  lista original)
             * Y aplicarle el nuevo filtro por calificación y el nuevo filtro por ubicación.
             */

            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");
            ViewBag.itemsPorPagina = itemsPorPagina;
            TempData["filtroPorCircular"] = circular;
            TempData["filtroPorDirecto"] = uno;
            TempData["filtroPorSubasta"] = subasta;

            var itemsFiltradosPorTrueque = aplicarFiltrosOrdenamiento();

            var itemsPaginados = itemsFiltradosPorTrueque.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = itemsPaginados;
            ViewBag.TipoOrdenamiento = (int)TempData.Peek("tipoOrdenamiento");

            return View("DesplegarResultadosDeBusqueda", itemsPaginados);
        }

        private List<InformacionItemBuscado> filtrarPorTipoTrueque(bool? subasta, bool? circular, bool? uno, List<InformacionItemBuscado> items)
        {
            var itemsFiltradosTrueque = new List<InformacionItemBuscado>();

            if (subasta == true)
            {
                foreach (var item in items)
                {
                    if (item.tipoTrueque.Equals("Subasta"))
                    {
                        itemsFiltradosTrueque.Add(item);
                    }
                }
            }
            if (circular == true)
            {
                foreach (var item in items)
                {
                    if (item.tipoTrueque.Equals("Circular"))
                    {
                        itemsFiltradosTrueque.Add(item);
                    }
                }
            }
            if (uno == true)
            {
                foreach (var item in items)
                {
                    if (item.tipoTrueque.Equals("Directo"))
                    {
                        itemsFiltradosTrueque.Add(item);
                    }
                }
            }
            ViewBag.uno = uno;
            ViewBag.circular = circular;
            ViewBag.subasta = subasta;
            return itemsFiltradosTrueque;
        }

        /*
            inicio de metodos para filtrar por estado del producto
        */

        public ActionResult filtrarResultadoDeBusquedaPorEstadoProducto(bool? nuevo, bool? casiNuevo, bool? bueno, bool? regular, bool? malo)
        {
            /*
             * Si la lista de los items con el estado actual tiene el filtro de calificación aplicado y se quiere aplicar un nuevo filtro, por ejemplo;
             * filtrar por ubicación, entonces hay que recuperar la lista sin el filtrado por estrellas actual (cuando no tenía filtro por calificación aplicado -  lista original)
             * Y aplicarle el nuevo filtro por calificación y el nuevo filtro por ubicación.
             */

            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");
            ViewBag.itemsPorPagina = itemsPorPagina;
            TempData["filtroNuevo"] = nuevo;
            TempData["filtroCasiNuevo"] = casiNuevo;
            TempData["filtroBueno"] = bueno;
            TempData["filtroRegular"] = regular;
            TempData["filtroMalo"] = malo;

            var itemsFiltradosPorTrueque = aplicarFiltrosOrdenamiento();

            var itemsPaginados = itemsFiltradosPorTrueque.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = itemsPaginados;
            ViewBag.TipoOrdenamiento = (int)TempData.Peek("tipoOrdenamiento");

            return View("DesplegarResultadosDeBusqueda", itemsPaginados);
        }

        private List<InformacionItemBuscado> filtrarPorEstadoProducto(bool? nuevo, bool? casiNuevo, bool? bueno, bool? regular, bool? malo, List<InformacionItemBuscado> items)
        {
            var itemsFiltradosTruequeEstado = new List<InformacionItemBuscado>();

            if (nuevo == true)
            {
                foreach (var item in items)
                {
                    var consultaEstadoProducto = (
                    from itemE in db.Item
                    where itemE.Id_Item == item.idItem
                    select itemE.Nombre_Estado_Item.Equals("Nuevo")
                    ).Single();

                    if (consultaEstadoProducto)
                    {
                        itemsFiltradosTruequeEstado.Add(item);
                    }
                }

            }

            if (casiNuevo == true)
            {
                foreach (var item in items)
                {
                    var consultaEstadoProducto = (
                    from itemE in db.Item
                    where itemE.Id_Item == item.idItem
                    select itemE.Nombre_Estado_Item.Equals("Casi nuevo")
                    ).Single();

                    if (consultaEstadoProducto)
                    {
                        itemsFiltradosTruequeEstado.Add(item);
                    }
                }

            }

            if (bueno == true)
            {
                foreach (var item in items)
                {
                    var consultaEstadoProducto = (
                    from itemE in db.Item
                    where itemE.Id_Item == item.idItem
                    select itemE.Nombre_Estado_Item.Equals("Bueno")
                    ).Single();

                    if (consultaEstadoProducto)
                    {
                        itemsFiltradosTruequeEstado.Add(item);
                    }
                }

            }

            if (regular == true)
            {
                foreach (var item in items)
                {
                    var consultaEstadoProducto = (
                    from itemE in db.Item
                    where itemE.Id_Item == item.idItem
                    select itemE.Nombre_Estado_Item.Equals("Regular")
                    ).Single();

                    if (consultaEstadoProducto)
                    {
                        itemsFiltradosTruequeEstado.Add(item);
                    }
                }

            }

            if (malo == true)
            {
                foreach (var item in items)
                {
                    var consultaEstadoProducto = (
                    from itemE in db.Item
                    where itemE.Id_Item == item.idItem
                    select itemE.Nombre_Estado_Item.Equals("Malo")
                    ).Single();

                    if (consultaEstadoProducto)
                    {
                        itemsFiltradosTruequeEstado.Add(item);
                    }
                }

            }
            ViewBag.nuevo = nuevo;
            ViewBag.casiNuevo = casiNuevo;
            ViewBag.bueno = bueno;
            ViewBag.regular = regular;
            ViewBag.malo = malo;

            return itemsFiltradosTruequeEstado;
        }

        /*
         Final de metodos de estado del producto 
        */

        public List<InformacionItemBuscado> aplicarFiltrosOrdenamiento()
        {
            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");
            ViewBag.itemsPorPagina = itemsPorPagina;
            List<InformacionItemBuscado> estadoActual = (List<InformacionItemBuscado>)TempData.Peek("ResultadosBusqueda");
            if ((bool)TempData.Peek("ordenoPorFecha") == true)
            {
                estadoActual = ordenarPorFechaPublicacion(estadoActual);
            }
            else if ((bool)TempData.Peek("ordenoPorCalificacion") == true)
            {
                estadoActual = ordenarPorCalificacion(estadoActual);
            }
            else if ((bool)TempData.Peek("ordenoPorUbicacion") == true)
            {
                estadoActual = ordenarPorUbicacion("Costa Rica", "San José", "Montes De Oca", estadoActual);
            }

            if ((bool)TempData.Peek("filtroPorCalificacion") == true)
                estadoActual = filtrarPorCalificacion((double)TempData.Peek("estrellasParaFiltro"), estadoActual);

            if ((bool?)TempData.Peek("filtroPorSubasta") == true || (bool?)TempData.Peek("filtroPorCircular") == true || (bool?)TempData.Peek("filtroPorDirecto") == true)
                estadoActual = filtrarPorTipoTrueque((bool?)TempData.Peek("filtroPorSubasta"), (bool?)TempData.Peek("filtroPorCircular"), (bool?)TempData.Peek("filtroPorDirecto"), estadoActual);

            if ((bool?)TempData.Peek("filtroNuevo") == true || (bool?)TempData.Peek("filtroCasiNuevo") == true || (bool?)TempData.Peek("filtroBueno") == true || (bool?)TempData.Peek("filtroRegular") == true || (bool?)TempData.Peek("filtroMalo") == true)
                estadoActual = filtrarPorEstadoProducto((bool?)TempData.Peek("filtroNuevo"), (bool?)TempData.Peek("filtroCasiNuevo"), (bool?)TempData.Peek("filtroBueno"), (bool?)TempData.Peek("filtroRegular"), (bool?)TempData.Peek("filtroMalo"), estadoActual);

            if ((bool)TempData.Peek("filtroPorUbicacion") == true)
                estadoActual = filtrarPorUbicacion((string)TempData.Peek("paisFiltro"), (string)TempData.Peek("provinciaFiltro"), (string)TempData.Peek("cantonFiltro"), estadoActual);

            ViewBag.uno = TempData.Peek("filtroPorDirecto");
            ViewBag.circular = TempData.Peek("filtroPorCircular");
            ViewBag.subasta = TempData.Peek("filtroPorSubasta");

            ViewBag.nuevo = TempData.Peek("filtroNuevo");
            ViewBag.casiNuevo = TempData.Peek("filtroCasiNuevo");
            ViewBag.bueno = TempData.Peek("filtroBueno");
            ViewBag.regular = TempData.Peek("filtroRegular");
            ViewBag.malo = TempData.Peek("filtroMalo");

            var itemsPaginados = estadoActual.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = itemsPaginados;
            TempData["estadoActualItems"] = estadoActual;
            return estadoActual;
        }

        public List<InformacionItemBuscado> ordenarPorCalificacion(List<InformacionItemBuscado> resultadosBusqueda)
        {
            var resultadosBusquedaOrdenados = resultadosBusqueda.OrderByDescending(i => i.calificacion).ToList();
            TempData["tipoOrdenamiento"] = 1;
            ViewBag.TipoOrdenamiento = 1;

            return resultadosBusquedaOrdenados;
        }

        public List<InformacionItemBuscado> ordenarPorFechaPublicacion(List<InformacionItemBuscado> resultadosBusqueda)
        {
            var resultadosBusquedaOrdenados = resultadosBusqueda.OrderByDescending(i => i.fechaPublicacion).ToList();
            TempData["tipoOrdenamiento"] = 3;
            ViewBag.TipoOrdenamiento = 3;

            return resultadosBusquedaOrdenados;
        }
        [Authorize]
        private List<InformacionItemBuscado> ordenarPorUbicacion(string Pais, string Provincia_Estado, string Canton_Ciudad, List<InformacionItemBuscado> estadoActualItems)
        {
            var resultadosBusquedaOrdenados = new List<InformacionItemBuscado>();

            foreach (var i in estadoActualItems)
            {
                if (i.pais == Pais && i.provincia_Estado == Provincia_Estado && i.canton_Ciudad == Canton_Ciudad)
                {
                    resultadosBusquedaOrdenados.Add(i);
                }
            }

            foreach (var i in estadoActualItems)
            {
                if (i.pais == Pais && i.provincia_Estado == Provincia_Estado && i.canton_Ciudad != Canton_Ciudad)
                {
                    resultadosBusquedaOrdenados.Add(i);
                }
            }

            foreach (var i in estadoActualItems)
            {
                if (i.pais == Pais && i.provincia_Estado != Provincia_Estado && i.canton_Ciudad != Canton_Ciudad)
                {
                    resultadosBusquedaOrdenados.Add(i);
                }
            }

            foreach (var i in estadoActualItems)
            {
                if (i.pais != Pais && i.provincia_Estado != Provincia_Estado && i.canton_Ciudad != Canton_Ciudad)
                {
                    resultadosBusquedaOrdenados.Add(i);
                }
            }

            TempData["tipoOrdenamiento"] = 2;
            ViewBag.TipoOrdenamiento = 2;

            return resultadosBusquedaOrdenados;
        }

        /*Como parte de la actividad se modifico este metodo que procesa la entrada de datos que viene del usuario
         * para que el caracter ";" no este considerado en la consulta a la base de datos.
        */
        private String normalizarString(String original)
        {
            char[] parametro = { ';' };
            String validado = original.Trim(parametro);
            return Regex.Replace(validado.ToLower(), @"\s", "");

        }


        private String tipoBusqueda(String criterio) //el criterio debería estar normalizado cuando se pase como parametro
        {
            String tipo = "";
            if (esCategoria(criterio))
            {
                tipo = "Categoria";
            }
            else if (esEtiqueta(criterio))
            {
                tipo = "Etiqueta";
            }
            else
            {
                tipo = "Titulo";
            }
            return tipo;
        }

        /*
        Actividad supervisada - Pair programming - Polina Tarassenko, Nicole Castillo, Fabian Jimenez

            Tareas técnicas
            1 - Revisar el estado de la recepcion de los datos por la barra de búsqueda y URL
            2 - Investigar sobre cuales cosas se deben evaluar en este tipo de entrada.
            3 - Agregar las validaciones
            
        */



        private List<InformacionItemBuscado> obtenerItems(string criterioBusqueda)
        {
            // Implementar objeto Nulo para devolver cuando la cadena es vacía.
            // De esta forma en la view se podrá hacer una comprobación para en tal caso presentar en pantalla que no se encontró nada.

            List<InformacionItemBuscado> items = new List<InformacionItemBuscado>();

            if (!criterioBusqueda.IsEmpty())
            {
                criterioBusqueda = this.normalizarString(criterioBusqueda);

                Comerciante comerciante = null;

                if (User != null)
                {
                    comerciante = db.Comerciante.Find(User.Identity.Name);
                }

                String tipo = tipoBusqueda(criterioBusqueda);

                if (comerciante != null)
                {
                    DateTime fecha = DateTime.Now;
                    db.insertarEnHistorialDeBusqueda(criterioBusqueda, tipo, fecha, comerciante.Email);
                }
                var consulta = obtenerInfoItems.buscarItems(tipo, criterioBusqueda, db);
                if (consulta.Any())
                {
                    foreach (var item in consulta) /*devuelve lista con informacion de los items para poder desplegarla en el view*/
                    {
                        if (obtenerInfoItems.obtenerTipoTrueque(item.Id_Item, db) != null && !(bool)(item.Eliminado))
                        {
                            items.Add(obtenerInfoItems.infoItemBuscado(item, db));
                        }
                    }

                }
            }
            return items;
        }



        /*ID historia: AVG_SRM29 Como usuario sin registrarse o truequeador quiero tener la posibilidad de desaplicar(limpiar) los filtros que apliqué previamente. 
         * Criterio de aceptacion: Dado [que se despliegue el resultado de la búsqueda de un ítem] cuando se produzca [que se aplique uno o varios filtros] entonces [se dará la opción de desaplicar el/los filtros por medio de un botón]. 
         * Tareas tecnicas realizadas: 
         PIG01-94	
                Incorporar un único botón para realizar la limpieza de los filtros.
            PIG01-95	
                Notificar al controlador cuando el botón es presionado.
            PIG01-96	
                Recuperar la lista de items original sin filtros aplicados.
            PIG01-97	
                Enviar la lista sin filtros a la vista.
            PIG01-98	
                Desplegar los resultados de búsqueda sin filtro en la vista.
         *  Devs: Fabian y Nicole.
         Complicaciones: se necesito paginar la lista y hacerla de este tipo ya que de lo contrario al devolver una lista
         simple al view da error.
         */
        public ActionResult limpiarFiltros()
        {
            int itemsPorPagina = (int)TempData.Peek("itemsPorPagina");
            int numeroPagina = (int)TempData.Peek("numeroPagina");
            TempData["filtroPorUbicacion"] = false;
            TempData["filtroPorCalificacion"] = false;
            TempData["filtroPorCircular"] = false;
            TempData["filtroPorDirecto"] = false;
            TempData["filtroPorSubasta"] = false;

            TempData["filtroNuevo"] = false;
            TempData["filtroCasiNuevo"] = false;
            TempData["filtroBueno"] = false;
            TempData["filtroRegular"] = false;
            TempData["filtroMalo"] = false;

            TempData["estrellas"] = 0;
            ViewBag.uno = TempData.Peek("filtroPorDirecto");
            ViewBag.circular = TempData.Peek("filtroPorCircular");
            ViewBag.subasta = TempData.Peek("filtroPorSubasta");

            ViewBag.nuevo = TempData.Peek("filtroNuevo");
            ViewBag.casiNuevo = TempData.Peek("filtroCasiNuevo");
            ViewBag.bueno = TempData.Peek("filtroBueno");
            ViewBag.regular = TempData.Peek("filtroRegular");
            ViewBag.malo = TempData.Peek("filtroMalo");

            var listaLimpia = aplicarFiltrosOrdenamiento();
            var itemsPaginados = listaLimpia.ToPagedList(numeroPagina, itemsPorPagina);
            ViewBag.resultadoBusQuedaPaginado = itemsPaginados;

            /* Debido a que los datos no se incertan directamente en la BD solo se hace la comparacion y el criterio de busqueda ya viene normalizado
             no hay necesidad de alguna validación adicional.*/
            return View("DesplegarResultadosDeBusqueda", itemsPaginados);
        }

        private void limpiarCoockies()
        {
            ViewBag.uno = false;
            ViewBag.circular = false;
            ViewBag.subasta = false;

            ViewBag.nuevo = false;
            ViewBag.casiNuevo = false;
            ViewBag.bueno = false;
            ViewBag.regular = false;
            ViewBag.malo = false;

            ViewBag.TipoOrdenamiento = 0;
            TempData["ordenoPorFecha"] = false;
            TempData["filtroPorUbicacion"] = false;
            TempData["filtroPorCalificacion"] = false;
            TempData["ordenoPorCalificacion"] = false;
            TempData["ordenoPorUbicacion"] = false;
            TempData["filtroPorCircular"] = false;
            TempData["filtroPorDirecto"] = false;
            TempData["filtroPorSubasta"] = false;

            TempData["filtroNuevo"] = false;
            TempData["filtroCasiNuevo"] = false;
            TempData["filtroBueno"] = false;
            TempData["filtroRegular"] = false;
            TempData["filtroMalo"] = false;
            TempData["tipoOrdenamiento"] = 0;
            TempData["estrellas"] = 0;
        }

        private void obtenerCoockies()
        {
            ViewBag.uno = TempData.Peek("filtroPorDirecto");
            ViewBag.circular = TempData.Peek("filtroPorCircular");
            ViewBag.subasta = TempData.Peek("filtroPorSubasta");
            ViewBag.nuevo = TempData.Peek("filtroNuevo");
            ViewBag.casiNuevo = TempData.Peek("filtroCasiNuevo");
            ViewBag.bueno = TempData.Peek("filtroBueno");
            ViewBag.regular = TempData.Peek("filtroRegular");
            ViewBag.malo = TempData.Peek("filtroMalo");
            ViewBag.TipoOrdenamiento = (int)TempData.Peek("tipoOrdenamiento");
        }
    }
}