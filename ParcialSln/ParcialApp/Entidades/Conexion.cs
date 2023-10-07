using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParcialApp.Entidades
{
    internal class Conexion
    {

        SqlConnection cnn = new SqlConnection(Properties.Resources.StringSql);
        SqlCommand cmd;
        SqlTransaction trans;

        public void Conectar()
        { cnn.Open();
          cmd = new SqlCommand();
          cmd.Connection = cnn;
        }


        public DataTable Consultar()
        {
            Conectar();
            cmd.CommandText = "SP_CONSULTAR_MATERIALES";
            cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            cnn.Close();
            return dt;


        }

        public bool GrabarOrden(OrdenRetiro orden)
        {
            bool resultado = false;
            trans = null;


            try 
            {
                Conectar();
                trans = cnn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.CommandText = "SP_INSERTAR_ORDEN";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@responsable",orden.Responsable);
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
                    cmd1.Parameters.AddWithValue("@nro_orden",nro_orden);
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
    }
}
