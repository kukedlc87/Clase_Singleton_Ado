using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Entidades
{
    public class Materiales
    {
        private int codigo;
        private string nombre;
        private double stock;

        public Materiales(int codigo, string nombre, double stock)
        {
            this.Codigo = codigo;
            this.Nombre = nombre;
            this.Stock = stock;
        }

        public Materiales()
        {
            codigo = 0;
            nombre = string.Empty;
            stock = 0;
        }

        public int Codigo { get => codigo; set => codigo = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public double Stock { get => stock; set => stock = value; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
