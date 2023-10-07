using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Entidades
{
    public class OrdenRetiro
    {

        private List<DetalleOrden> detOrd;
        private int nroOrden;
        private DateTime fecha;
        private string responsable;

        public OrdenRetiro(List<DetalleOrden> detOrd, int nroOrden, DateTime fecha, string id_responsable)
        {
            DetOrd = detOrd;
            this.NroOrden = nroOrden;
            this.Fecha = fecha;
            this.Responsable = id_responsable;
        }

        public OrdenRetiro()
        {
            fecha = DateTime.Now;
            detOrd = new List<DetalleOrden>();
        }

        public int NroOrden { get => nroOrden; set => nroOrden = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }
        public string Responsable { get => responsable; set => responsable = value; }
        internal List<DetalleOrden> DetOrd { get => detOrd; set => detOrd = value; }



        public void AgregarDetalle(DetalleOrden detalle)
        {
            DetOrd.Add(detalle);
        }


        public void QuitarDetalle(int posicion)
        { 

        DetOrd.RemoveAt(posicion);

        }



    }
}
