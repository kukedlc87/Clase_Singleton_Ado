using ParcialApp.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Servicios.Interfaz
{
    public interface IServicio
    {
        bool GrabarOrden(OrdenRetiro orden);

        List<Materiales> Consultar();



    }
}
