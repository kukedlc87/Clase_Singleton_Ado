using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcialApp.Acceso_a_datos_
{
    public class HelperDao
    {
        private static HelperDao instancia;
        private SqlConnection cnn;


        private HelperDao()
        { 
            cnn = new SqlConnection(Properties.Resources.StringSql);
        
        
        }


        public static HelperDao ObtenerInstancia() 
        {
            if (instancia == null)
            { 
                instancia = new HelperDao();
            
            }
            return instancia;

        }

        public SqlConnection ObtenerConexion()
        { 
        
            return this.cnn;
        
        }




    }
}
