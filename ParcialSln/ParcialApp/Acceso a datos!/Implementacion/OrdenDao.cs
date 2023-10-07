using ParcialApp.Acceso_a_datos_.Interfaz;
using ParcialApp.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Acceso_a_datos_.Implementacion
{
    public class OrdenDao : IOrdenDao
    {
        public bool GrabarOrden(OrdenRetiro orden)
        {
            SqlTransaction trans;
            bool resultado = false;
            trans = null;
            SqlConnection cnn = HelperDao.ObtenerInstancia().ObtenerConexion();


            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                trans = cnn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandText = "SP_INSERTAR_ORDEN";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@responsable", orden.Responsable);
                SqlParameter param = new SqlParameter();
                param.ParameterName = "nro";
                param.SqlDbType = SqlDbType.Int;
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
                int nro_orden = (int)param.Value;

                SqlCommand cmd1;

                //@nro_orden
                //@detalle i
                //@codigo in
                //@cantidad
                int nro_detalle = 1;
                foreach (DetalleOrden det in orden.DetOrd)
                {
                    cmd1 = new SqlCommand("SP_INSERTAR_DETALLES", cnn, trans);
                    cmd1.Connection = cnn;
                    cmd1.Transaction = trans;
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@nro_orden", nro_orden);
                    cmd1.Parameters.AddWithValue("@detalle", nro_detalle);
                    cmd1.Parameters.AddWithValue("@codigo", det.Material.Codigo);
                    cmd1.Parameters.AddWithValue("@cantidad", det.Cantidad);
                    cmd1.ExecuteNonQuery();
                    nro_detalle++;
                }

                trans.Commit();
                resultado = true;
            }
            catch
            {
                if (trans != null)
                {
                    trans.Rollback();
                    resultado = false;
                    throw;
                }

            }
            finally
            {
                cnn.Close();
            }

            return resultado;
        }

        public List<Materiales> Consultar()
        {
            SqlConnection cnn =  HelperDao.ObtenerInstancia().ObtenerConexion();
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "SP_CONSULTAR_MATERIALES";
            cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            List<Materiales> lstm = new List<Materiales>();
            foreach (DataRow fila in dt.Rows)
            {
                int codigo = Convert.ToInt32(fila[0]);
                string nombre = fila[1].ToString();
                int stock = Convert.ToInt32(fila[2]);
                Materiales m = new Materiales(codigo, nombre, stock);
                lstm.Add(m);




            }
            cnn.Close();
            return lstm;


        }

      
    }
}
