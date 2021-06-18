using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_TruequesUCR.Models;
using PagedList;
using System.IO;
using System.Drawing;

namespace Proyecto_TruequesUCR.Controllers
{
    public class HomeController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();
        public ActionResult Index(int? page, int? numItem)
        {
            int numberItems = numItem ?? 6;
            int numberPage = page ?? 1;
            if (page <= 0 || page >= Int32.MaxValue || numItem <= 0 || numItem >= Int32.MaxValue)
            {
                numberItems = 6;
                numberPage = 1;
            }
            var paginaDeProductos = obtenerItems().ToPagedList(numberPage, numberItems);

            ViewBag.onePageOfProducts = paginaDeProductos;
            ViewBag.numeroItems = numItem;
            return View("Index", paginaDeProductos);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private List<InformacionItemBuscado> obtenerItems()
        {
            // Implementar objeto Nulo para devolver cuando la cadena es vacía.
            // De esta forma en la view se podrá hacer una comprobación para en tal caso presentar en pantalla que no se encontró nada.

            List<InformacionItemBuscado> items = new List<InformacionItemBuscado>();

            var consulta =
            from item in db.Item
            orderby item.Fecha descending
            select item;

            foreach (var item in consulta)
            {
                if (obtenerTipoTrueque(item.Id_Item) != null && !(bool)(item.Eliminado))
                {
                    String nombre = obtenerNombreItem(item.Id_Item, item.Email_Comerciante);
                    String descripcion = obtenerDescripcionItem(item.Id_Item, item.Email_Comerciante);
                    String nombreComerciante = obtenerNombreComerciante(item.Email_Comerciante);
                    String fecha = obtenerFechaPublicacion(item.Id_Item, item.Email_Comerciante);
                    byte[] foto = obtenerDireccionImagen(item.Id_Item, item.Email_Comerciante);
                    String imagen = "";
                    if(foto != null)
                    {
                        String img64 = Convert.ToBase64String(foto);
                        String img64url = string.Format("data:image/" + "jpeg" + ";base64,{0}", img64);
                        imagen = img64url;
                    }
                    double calificacion = obtenerCalificacionItem(item.Email_Comerciante);
                    String pais = obtenerPais(item.Email_Comerciante);
                    String provincia_estado = obtenerProvincia_Estado(item.Email_Comerciante);
                    String canton_ciudad = obtenerCanton_Ciudad(item.Email_Comerciante);
                    String fechaExp = obtenerFechaExp(item.Email_Comerciante);
                    String tipoTrueque = obtenerTipoTrueque(item.Id_Item);
                    int idTrueque = (int)item.IdTrueque;
                    items.Add(new InformacionItemBuscado(nombre, descripcion, nombreComerciante, fecha, calificacion, pais, canton_ciudad, provincia_estado, fechaExp, tipoTrueque, imagen, idTrueque, item.Id_Item, item.Nombre_Categoria_Item));

                }
            }
          
            return items;
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

            if (pais == null) {
                pais = "Pais.";
            }

            return pais.ToString();
        }

        private string obtenerFechaExp(string emailComerciante)
        {
            var fechaExp =
                (
                from item in db.Item
                join trueque in db.Trueques on item.IdTrueque equals trueque.IdTrueque
                where (item.Email_Comerciante == emailComerciante)
                select trueque.FechaFinal
                ).FirstOrDefault();
            return fechaExp.ToString();
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
                p_e = "Provincia/Estado";
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
                cc = "Canton/Ciudad";
            }

            return cc.ToString();
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

        private string obtenerNombreItem(int id, string emailComerciante)
        {
            var nombre = db.obtenerNombreItem(id, emailComerciante).Single();
            return nombre;
        }

        private double obtenerCalificacionItem(string emailComerciante)
        {
            double cal =
                 (
                 from item in db.Item
                 join calificacion in db.Calificacion on item.Email_Comerciante equals calificacion.Email_Comerciante
                 where (calificacion.Email_Comerciante == emailComerciante)
                 select calificacion.Calificacion_Comerciante
                 ).FirstOrDefault();

            return cal;
        }

        private string obtenerDescripcionItem(int iditem, string emailComerciante)
        {
            var descripcion = db.obtenerDescripcionItem(iditem, emailComerciante).Single();

            return descripcion;
        }

        private string obtenerNombreComerciante(string emailComerciante)
        {

            var nombreComerciante =
                (from comerciante in db.Comerciante
                 where (comerciante.Email == emailComerciante)
                 select (comerciante.Nombre + " " + comerciante.Apellido)
                ).Single();

            return nombreComerciante;
        }
        private string obtenerTipoTrueque(int id) //si devuelve 1 es circular, si devuelve 2 es directo y si devuelve 3 es subasta
        {
            var tipoTrueque =
                (
                from item in db.Item
                join trueque in db.Trueques on item.IdTrueque equals trueque.IdTrueque
                where (item.Id_Item == id)
                select trueque.Tipo
                ).FirstOrDefault();

            if (tipoTrueque == null)
            {
                return null;
            }

            return tipoTrueque;
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

        
    }

}