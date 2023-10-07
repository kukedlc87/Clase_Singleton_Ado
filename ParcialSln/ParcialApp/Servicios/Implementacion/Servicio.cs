using ParcialApp.Acceso_a_datos_.Implementacion;
using ParcialApp.Acceso_a_datos_.Interfaz;
using ParcialApp.Entidades;
using ParcialApp.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Servicios.Implementacion
{
    public class Servicio : IServicio
    {
        private IOrdenDao dao;

        public Servicio()
        {

            dao = new OrdenDao();
        
        }

        public List<Materiales> Consultar()
        {
            return dao.Consultar();
        }

        public bool GrabarOrden(OrdenRetiro orden)
        {
            return dao.GrabarOrden(orden);
        }
    }
}
