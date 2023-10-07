using ParcialApp.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Acceso_a_datos_.Interfaz
{
    public interface IOrdenDao
    {
        List<Materiales> Consultar();
        bool GrabarOrden(OrdenRetiro orden);
    }
}
