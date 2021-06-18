using Proyecto_TruequesUCR.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Proyecto_TruequesUCR.Controllers
{
    public class RecomendacionPorBusquedaActualController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();
        [HttpGet]
        //for Filter parital view
        [ChildActionOnly]
        public ActionResult Index()
        {
            var criterio = (string)TempData.Peek("criterioBusqueda");
            var recomendaciones = obtenerListaRecomendaciones(criterio);

            TempData["recomendaciones"] = recomendaciones;
            TempData.Keep();

            return View("DesplegarResultadosDeBusqueda");
        }

        private List<RecomendacionPorBusquedaActual> obtenerListaRecomendaciones(string criterio)
        {
            // ToDo diferenciar entre tipos de búsqueda.

            var categoriasAsociadas = obtenerListaDeCategoriasAsociadas(criterio);
            var recomendacionesCategoria = obtenerRecomendacionesCategorias(categoriasAsociadas);
            return recomendacionesCategoria;
        }

        private List<RecomendacionPorBusquedaActual> obtenerRecomendacionesCategorias(List<String> categoriasHermanas)
        {
            List<RecomendacionPorBusquedaActual> recomendaciones = new List<RecomendacionPorBusquedaActual>();

            var items =
            from item in db.Item
            select item;

            foreach (var item in items)
            {
                if ((obtenerTipoTrueque(item.Id_Item) != null) && !(bool)(item.Eliminado))
                {
                    if (categoriasHermanas.Contains(item.Categoria.Nombre_Categoria))
                    {
                        recomendaciones.Add(new RecomendacionPorBusquedaActual(item.Titulo, obtenerImagen(item), item.Id_Item, obtenerPais(item.Email_Comerciante), obtenerProvincia_Estado(item.Email_Comerciante),
                            obtenerCanton_Ciudad(item.Email_Comerciante), obtenerFechaPublicacion(item.Id_Item, item.Email_Comerciante), obtenerCalificacionItem(item.Email_Comerciante), obtenerNombreComerciante(item.Email_Comerciante), item.Email_Comerciante));
                    }
                }
            }

            return recomendaciones;
        }

        private List<RecomendacionPorBusquedaActual> obtenerRecomendacionesEtiqueta(string etiqueta)
        {
            List<RecomendacionPorBusquedaActual> recomendaciones = new List<RecomendacionPorBusquedaActual>();

            // ToDo

            return recomendaciones;
        }

        private string obtenerNombreItem(int id, string emailComerciante)
        {
            var nombre = db.obtenerNombreItem(id, emailComerciante).Single();
            return nombre;
        }

        private byte[] obtenerDireccionImagen(int id, string emailComerciante)
        {
            byte[] f =
                 (
                 from comerciante in db.Comerciante
                 join item in db.Item on comerciante.Email equals item.Email_Comerciante
                 join foto in db.Fotos on item.Id_Item equals foto.Id_Item_Fotos
                 where (foto.Id_Item_Fotos == id && foto.Email_Item_Fotos.Equals(emailComerciante))
                 select foto.Foto
                 ).FirstOrDefault();
            return f;
        }

        private String obtenerImagen(Item item)
        {
            byte[] foto = obtenerDireccionImagen(item.Id_Item, item.Email_Comerciante);
            String imagen = "";
            if (foto != null)
            {
                String img64 = Convert.ToBase64String(foto);
                String img64url = string.Format("data:image/" + "jpeg" + ";base64,{0}", img64);
                imagen = img64url;
            }

            return imagen;
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

        private List<String> obtenerListaDeCategoriasAsociadas(String categoria)
        {
            /* 
             * Buscar en el árbol de categorías, la categoría que tenga como hija a la categoría recibida por parámetro.
             * En el caso de que se quiera subir 2 niveles se tendría que buscar la categoría abuelo.
             * 
             * Una vez identificada la categoría padre, buscar todas las categorías hijas; es decir buscar entre todas las categorías 
             * las que tengan como padre a la categoría identificada como padre. En resumen hacer recomendaciones de categorías hermanas
             * a la 
             * 
             * Agregar cada categoría hija a la lista de cateogrías asociadas.
             * 
             * Una vez con la lista de categorías agregar a una lista de items los que tengan la categoría hermana.
             * 
             *                                              Celular
             *                                             /
             *                                          Iphone
             *                   /                       |                 \            \
             *              Iphone 11 - Iphone 8 .... IphoneX .... Cargador Iphone, Estuche Iphone....
             *              
             * ¿Cuántos niveles hay que bajar?             
             */

            var categoriaPadre =
                (from c in db.Categoria
                 where c.Nombre_Categoria.Equals(categoria)
                 select c.Padre).SingleOrDefault();

            var categoriasHermanas =
                (from h in db.Categoria
                 where h.Padre.Equals(categoriaPadre)
                 select h.Nombre_Categoria
                ).ToList();


            categoriasHermanas.Remove(categoria);
            return categoriasHermanas;
        }

        private string obtenerNombreComerciante(string emailComerciante)
        {
            var nombreComerciante =
                (
                  from comerciante in db.Comerciante
                  where (comerciante.Email == emailComerciante)
                  select (comerciante.Nombre + " " + comerciante.Apellido)
                ).Single();

            return nombreComerciante;
        }

        private String obtenerFechaPublicacion(int id, string emailComerciante)
        {
            var fecha =
                (
                from item in db.Item
                where (item.Id_Item == id && item.Email_Comerciante == emailComerciante)
                select item.Fecha
                ).Single();

            return fecha.ToString();
        }

        private double obtenerCalificacionItem(string emailComerciante)
        {
            var cal =
                 (
                 from item in db.Item
                 join calificacion in db.Calificacion on item.Email_Comerciante equals calificacion.Email_Comerciante
                 where (calificacion.Email_Comerciante == emailComerciante)
                 select calificacion.Calificacion_Comerciante
                 ).FirstOrDefault();
            if (cal == 0.0)
            {
                cal = 0.0;
            }

            return cal;
        }

        private string obtenerPais(string emailComerciante)
        {
            var pais =
                 (
                 from perfil in db.Perfil
                 join comerciante in db.Comerciante on perfil.ComercianteEmail equals comerciante.Email
                 where (perfil.ComercianteEmail == emailComerciante)
                 select perfil.Pais
                 ).FirstOrDefault();

            if (pais == null)
            {
                return null;
            }

            return pais.ToString();
        }

        private string obtenerProvincia_Estado(string emailComerciante)
        {
            var p_e =
                  (
                  from item in db.Item
                  join Perfil in db.Perfil on item.Email_Comerciante equals Perfil.ComercianteEmail
                  where (Perfil.ComercianteEmail == emailComerciante)
                  select Perfil.Provincia_Estado
                  ).FirstOrDefault();

            if (p_e == null)
            {
                return null;
            }

            return p_e.ToString();
        }

        private string obtenerCanton_Ciudad(string emailComerciante)
        {
            var cc =
                  (
                  from item in db.Item
                  join Perfil in db.Perfil on item.Email_Comerciante equals Perfil.ComercianteEmail
                  where (Perfil.ComercianteEmail == emailComerciante)
                  select Perfil.Canton_Ciudad
                  ).FirstOrDefault();

            if (cc == null)
            {
                return null;
            }

            return cc.ToString();
        }
    }
}




