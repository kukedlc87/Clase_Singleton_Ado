using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Entidades
{
    public class DetalleOrden
    {
        private Materiales material;
        private int cantidad;

        public DetalleOrden(Materiales material, int cantidad)
        {
            this.Material = material;
            this.Cantidad = cantidad;
        }

        public DetalleOrden()
        {
            cantidad = 0;
             material = null;
        }

        public int Cantidad { get => cantidad; set => cantidad = value; }
        internal Materiales Material { get => material; set => material = value; }
    }
}
