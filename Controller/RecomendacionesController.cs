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

namespace Proyecto_TruequesUCR.Controllers
{
    public class RecomendacionesController : Controller
    {
        //private ProyectoEntities db = new ProyectoEntities();
        private ProyectoEntities db;
        private ObtenerInfoItems infoBusqueda = new ObtenerInfoItems();

        public RecomendacionesController()
        {
            db = new ProyectoEntities();
        }

        public RecomendacionesController(ProyectoEntities db)
        {
            this.db = db;
        }

        [Authorize]
        public ActionResult RecomendacionesPorGustos(int? page, int? numItem)
        {
            Comerciante comerciante;
            if (User != null)
            {
                comerciante = db.Comerciante.Find(User.Identity.Name);
            }
            else
            {
                comerciante = db.Comerciante.Find("default@gmail.com");
            }

            int numberItems = numItem ?? 6;
            int numberPage = page ?? 1;
            if (page <= 0 && page >= Int32.MaxValue && numItem <= 0 && numItem >= Int32.MaxValue)
            {
                numberItems = 6;
                numberPage = 1;
            }

            String email = comerciante.Email;
            IOrderedQueryable<Item> itemsconsulta = getItems();
            List<InformacionItemBuscado> recomendaciones = obtenerListaDeItemsPorEtiquetas(email, obtenerEtiquetaGustos(email), itemsconsulta).Union(obtenerListaDeItemsPorCategorias(email, obtenerCategoriaGustos(email), itemsconsulta)).ToList();

            var recomendacionesPaginado = recomendaciones.ToPagedList(numberPage, numberItems);

            ViewBag.onePageOfProducts = recomendacionesPaginado;
            ViewBag.numeroItems = numItem;

            return View("RecomendacionesPorGustos", recomendacionesPaginado);
        }

        private List<InformacionItemBuscado> obtenerListaDeItemsPorEtiquetas(string email, List<Etiquetas> listaEtiquetas, IOrderedQueryable<Item> items)
        {

            List<InformacionItemBuscado> recomendacionesPorEtiquetas = new List<InformacionItemBuscado>();
            List<Etiquetas> etiquetasDelComerciante = listaEtiquetas;//obtenerEtiquetaGustos(email);

            var consulta = items;//getItems();

            foreach (var item in consulta)
            {
                List<Etiquetas> etiquetasDeItem = item.Etiquetas.ToList();
                bool hasMatch = etiquetasDeItem.Intersect(etiquetasDelComerciante).Any();

                if (hasMatch && (obtenerTipoTrueque(item.Id_Item) != null) && !(bool)(item.Eliminado) && item.Email_Comerciante != email)
                {

                    recomendacionesPorEtiquetas.Add(infoBusqueda.infoItemBuscado(item, db));

                }

            }


            return recomendacionesPorEtiquetas;
        }


        [Authorize]
        public ActionResult RecomendacionesPorHistorialBusqueda(int? page, int? numItem)
        {
            Comerciante comerciante;
            if (User != null)
            {
                comerciante = db.Comerciante.Find(User.Identity.Name);
            }
            else
            {
                comerciante = db.Comerciante.Find("default@gmail.com");
            }

            int numberItems = numItem ?? 6;
            int numberPage = page ?? 1;
            if (page <= 0 && page >= Int32.MaxValue && numItem <= 0 && numItem >= Int32.MaxValue)
            {
                numberItems = 6;
                numberPage = 1;
            }

            String email = comerciante.Email;
            List<InformacionItemBuscado> recomendaciones = obtenerListaDeItemsHistorialBusqueda(email);
            var recomendacionesPaginado = recomendaciones.ToPagedList(numberPage, numberItems);

            ViewBag.onePageOfProducts = recomendacionesPaginado;
            ViewBag.numeroItems = numItem;

            return View("RecomendacionesPorHistorialBusqueda", recomendacionesPaginado);
        }


        [Authorize]
        public ActionResult RecomendacionesPorHistorialTrueques(int? page, int? numItem)
        {
            Comerciante comerciante;
            if (User != null)
            {
                comerciante = db.Comerciante.Find(User.Identity.Name);
            }
            else
            {
                comerciante = db.Comerciante.Find("default@gmail.com");
            }

            int numberItems = numItem ?? 6;
            int numberPage = page ?? 1;
            if (page <= 0 && page >= Int32.MaxValue && numItem <= 0 && numItem >= Int32.MaxValue)
            {
                numberItems = 6;
                numberPage = 1;
            }

            String email = comerciante.Email;

            var listaIdsTruequesUsuario =
                (from h in db.HistorialTrueques
                 where h.EmailComerciante == email
                 select h.IdTrueque).ToList();

            var trueques = from t in db.Trueques select t;
            var listaIdsTruequesFinalizados = new List<int>();


            // Genera una lista de enteros con los id's de truques del usuario actual que hayan sido finalizados.
            foreach (var Idtrueque in listaIdsTruequesUsuario)
            {
                if (trueques.Where(t => t.IdTrueque == Idtrueque && t.Estado.Equals("Finalizado")).Any())
                {
                    listaIdsTruequesFinalizados.Add(Idtrueque);
                }
            }
            // Obtener la lista de id items que el usuario ha recibido de intercambios.
            var items = from i in db.Item select i;
            var idItemsRecibidosEnIntercambio = new List<int>();
            foreach (var IdTrueque in listaIdsTruequesFinalizados)
            {
                if (items.Where(i => i.IdTrueque == IdTrueque).Any())
                {
                    var consulta = items.Where(i => i.IdTrueque == IdTrueque).ToList();
                    foreach (var c in consulta)
                    {
                        if (c.Email_Comerciante != email) { 
                            idItemsRecibidosEnIntercambio.Add(c.Id_Item);
                        }
                       
                    }
                }
            }


            List<InformacionItemBuscado> recomendaciones = obtenerListaItemsPorHistorialTrueques(email, idItemsRecibidosEnIntercambio);
            var recomendacionesPaginado = recomendaciones.ToPagedList(numberPage, numberItems);

            ViewBag.onePageOfProducts = recomendacionesPaginado;
            ViewBag.numeroItems = numItem;

            return View("RecomendacionesPorHistorialBusqueda", recomendacionesPaginado);
        }

        private List<InformacionItemBuscado> obtenerListaItemsPorHistorialTrueques(string email, List<int> idsItemsRecibidosEnIntercambio)
        {
            // A partir de la lista de id conseguir la lista de items
            List<Item> items = new List<Item>();

            // Obtener lista de items asociados a los id's.
            foreach (var id in idsItemsRecibidosEnIntercambio)
            {
                items.Add(db.Item.Where(i => i.Id_Item == id).FirstOrDefault());
            }

            List<Categoria> lista = obtenerListaCategorias(items);
            IOrderedQueryable<Item> itemsconsulta = getItems();
            List<InformacionItemBuscado> resultPorCategorias = obtenerListaDeItemsPorCategorias(email, lista, itemsconsulta);
            List<Etiquetas> listaEtiquetas = obtenerListaEtiquetas(items);
            List<InformacionItemBuscado> resultPorEtiquetas = obtenerListaDeItemsPorEtiquetas(email, listaEtiquetas, itemsconsulta);
            List<InformacionItemBuscado> result = unirListasCategoriasEtiquetas(resultPorEtiquetas, resultPorCategorias, email);

            return result;
        }

        private List<InformacionItemBuscado> obtenerListaDeItemsHistorialBusqueda(string email)
        {
            List<String> historial = obtenerListaDeItemsPorCantAparicionesHistorialDeBusqueda(email).Union(obtenerListaDeItemsPorFechaHistorialDeBusqueda(email)).ToList();

            List<Item> items = new List<Item>();

            foreach (var busqueda in historial)
            {
                String tipo = consultaHistorial(email, busqueda);


                items = items.Union(infoBusqueda.buscarItems(tipo, busqueda, db)).ToList();
            }
            List<Categoria> lista = obtenerListaCategorias(items);
            IOrderedQueryable<Item> itemsconsulta = getItems();
            List<InformacionItemBuscado> resultPorCategorias = obtenerListaDeItemsPorCategorias(email, lista, itemsconsulta);
            List<Etiquetas> listaEtiquetas = obtenerListaEtiquetas(items);
            List<InformacionItemBuscado> resultPorEtiquetas = obtenerListaDeItemsPorEtiquetas(email, listaEtiquetas, itemsconsulta);
            List<InformacionItemBuscado> result = unirListasCategoriasEtiquetas(resultPorEtiquetas, resultPorCategorias, email);

            return result;
        }

        private List<InformacionItemBuscado> unirListasCategoriasEtiquetas(List<InformacionItemBuscado> listaItemsEtiquetas, List<InformacionItemBuscado> listaItemsCategorias, String email)
        {
            List<InformacionItemBuscado> result = new List<InformacionItemBuscado>();
            for (int i = 0; i < listaItemsEtiquetas.Count(); i++)
            {
                bool insertar = true;
                for (int j = 0; j < result.Count(); j++)
                {
                    if (listaItemsEtiquetas.ElementAt(i).idItem == result.ElementAt(j).idItem)
                    {
                        insertar = false;
                        break;
                    }
                }
                if (insertar)
                {
                    result.Add(listaItemsEtiquetas.ElementAt(i));
                }
            }
            for (int i = 0; i < listaItemsCategorias.Count(); i++)
            {
                bool insertar = true;
                for (int j = 0; j < result.Count(); j++)
                {
                    if (listaItemsCategorias.ElementAt(i).idItem == result.ElementAt(j).idItem)
                    {
                        insertar = false;
                        break;
                    }
                }
                if (insertar)
                {
                    result.Add(listaItemsCategorias.ElementAt(i));
                }
            }
            return result;
        }

        private String consultaHistorial(string email, string busq)
        {
            var consulta =
           (from busqueda in db.HistorialBusquedas
            where busqueda.EmailComerciante.Equals(email) && busqueda.Descripcion.Equals(busq)
            select busqueda.Tipo).SingleOrDefault();

            return consulta;
        }
        private List<String> obtenerListaDeItemsPorCantAparicionesHistorialDeBusqueda(string email)
        {
            var consulta =
            from busqueda in db.HistorialBusquedas
            orderby busqueda.CantAparicion descending
            where busqueda.EmailComerciante == email
            select busqueda.Descripcion;

            List<String> listaConsulta = consulta.ToList();
            List<String> lista = new List<String>();
            int size = listaConsulta.Count();
            if (size >= 5/* este numero puede cambiar dependiendo de lo que diga el PO*/)
            {
                return (listaConsulta).GetRange(0, 5);
            }
            return listaConsulta;
        }

        private List<String> obtenerListaDeItemsPorFechaHistorialDeBusqueda(string email)
        {
            var consulta =
            from busqueda in db.HistorialBusquedas
            orderby busqueda.FechaYHora descending
            where busqueda.EmailComerciante == email
            select busqueda.Descripcion;
            List<String> listaConsulta = consulta.ToList();
            List<String> lista = new List<String>();
            int size = listaConsulta.Count();
            if (size >= 5/* este numero puede cambiar dependiendo de lo que diga el PO*/)
            {
                return (listaConsulta).GetRange(0, 5);
            }
            return listaConsulta;
        }

        private List<Categoria> obtenerListaCategorias(List<Item> listaItems)
        {
            int size = listaItems.Count();
            List<Categoria> lista = new List<Categoria>();
            if (size >= 10)
            {
                for (int ind = 0; ind < size; ++ind)//consultar numero con PO
                {

                    if (!lista.Contains(listaItems[ind].Categoria))
                    {
                        lista.Add(listaItems[ind].Categoria);

                    }
                    if (lista.Count() == 10)
                    {
                        break;
                    }
                }

            }
            else
            {
                for (int ind = 0; ind < size; ++ind)
                {

                    lista.Add(listaItems[ind].Categoria);
                }
            }

            return lista;
        }


        private List<Etiquetas> obtenerListaEtiquetas(List<Item> listaItems)
        {
            int size = listaItems.Count();
            List<Etiquetas> lista = new List<Etiquetas>();
            if (size >= 5)//consultar numero con PO
            {
                for (int ind = 0; ind < size; ++ind)
                {
                    var listaEtiquetasItem = listaItems[ind].Etiquetas.ToList();

                    for (int i = 0; i < listaItems[ind].Etiquetas.Count(); ++i)
                    {

                        if (lista.Count() == 5)
                        {
                            break;
                        }

                        if (!lista.Contains(listaEtiquetasItem[i]))
                        {
                            lista.Add(listaEtiquetasItem[i]);

                        }
                    }

                }

            }
            else
            {
                for (int ind = 0; ind < size; ++ind)
                {

                    var listaEtiquetasItem = listaItems[ind].Etiquetas.ToList();

                    for (int i = 0; i < listaItems[ind].Etiquetas.Count(); ++i)
                    {

                        if (!lista.Contains(listaEtiquetasItem[i]))
                        {
                            lista.Add(listaEtiquetasItem[i]);

                        }
                    }
                }
            }

            return lista;
        }

        private List<InformacionItemBuscado> obtenerListaDeItemsPorCategorias(string email, List<Categoria> lista, IOrderedQueryable<Item> items)
        {

            List<InformacionItemBuscado> recomendacionesPorCategorias = new List<InformacionItemBuscado>();
            List<Categoria> categorias = lista;//obtenerCategoriaGustos(email);

            var consulta = items;//getItems();

            foreach (var item in consulta)
            {

                bool hasMatch = categorias.Contains(item.Categoria);

                if (hasMatch && (obtenerTipoTrueque(item.Id_Item) != null) && !(bool)(item.Eliminado) && item.Email_Comerciante != email)
                {

                    recomendacionesPorCategorias.Add(infoBusqueda.infoItemBuscado(item, db));

                }

            }


            return recomendacionesPorCategorias;
        }

        private IOrderedQueryable<Item> getItems()
        {
            var consulta =
            from item in db.Item
            orderby item.Fecha descending
            select item;

            return consulta;
        }

        [Authorize]
        private List<Categoria> obtenerCategoriaGustos(string emailComerciante)
        {
            var comerciante = (
                 from c in db.Comerciante
                 where c.Email == emailComerciante
                 select c
                ).SingleOrDefault();

            return comerciante.Categoria.ToList();
        }

        [Authorize]
        private List<Etiquetas> obtenerEtiquetaGustos(string emailComerciante)
        {
            var comerciante = (
                 from c in db.Comerciante
                 where c.Email == emailComerciante
                 select c
                ).SingleOrDefault();

            return comerciante.Etiquetas.ToList();
        }




        private string obtenerTipoTrueque(int id)
        {
            var tipoTrueque =
                (
                from item in db.Item
                join trueque in db.Trueques on item.IdTrueque equals trueque.IdTrueque
                where (item.Id_Item == id)
                select trueque.Tipo
                ).SingleOrDefault();

            return tipoTrueque;
        }


    }
}