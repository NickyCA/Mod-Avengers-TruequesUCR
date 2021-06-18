using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_TruequesUCR.Models
{
    public class RecomendacionPorBusquedaActual
    {

        public String nombreItem { get; set; }
        public String imagen { get; set; }
        public int idItem { get; set; }
        public String pais { get; set; }
        public String provincia { get; set; }
        public String ciudad { get; set; }
        public String fecha { get; set; }
        public double calificacion { get; set; }
        public String nombreComerciante { get; set; }
        public String emailComerciante { get; set; }

        public RecomendacionPorBusquedaActual(String nombre, String imagen, int idItem)
        {
            this.nombreItem = nombre;
            this.imagen = imagen;
            this.idItem = idItem;
        }

        public RecomendacionPorBusquedaActual(String nombre,String imagen, int idItem, String pais, String provincia, String ciudad, String fecha, double calificacion, String nombreComerciante, String email)
        {
            this.nombreItem = nombre;
            this.imagen = imagen;
            this.idItem = idItem;
            this.pais = pais;
            this.provincia = provincia;
            this.ciudad = ciudad;
            this.fecha = fecha;
            this.calificacion = calificacion;
            this.nombreComerciante = nombreComerciante;
            this.emailComerciante = email;
        }
    }
}