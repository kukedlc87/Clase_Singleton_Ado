using ParcialApp.Acceso_a_datos.Implementacion;
using ParcialApp.Acceso_a_datos.Interfaz;
using ParcialApp.Entidades;
using ParcialApp.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//servicios/Iservicio/interfaz
public interface Iservicio
{
    bool AgregarOrden(OrdenRetiro orden);

    List<Materiales> TraerMateriales();

}

//servicios/servicio/Implementacion

public class Servicio : Iservicio
{
    private IOrdenDao dao;

    public Servicio()
    {
        dao = new OrdenRetiroDao();

    }
    public bool AgregarOrden(OrdenRetiro orden)
    {
        return dao.AgregarOrden(orden);
    }

    public List<Materiales> TraerMateriales()
    {
        return dao.TraerMateriales();
    }
}

//frm_alta

public partial class Frm_Alta : Form
{

    OrdenRetiro NewOrder;
    Iservicio servicio = null;

    public Frm_Alta()
    {
        //InitializeComponent();

        NewOrder = new OrdenRetiro();
        servicio = new Servicio();

    }




    private void btnAceptar_Click(object sender, EventArgs e)
    {
        //validaciones
        if (string.IsNullOrEmpty(txtResp.Text))
        { MessageBox.Show("Debe agregar nombre de responsable!"); return; }

        if (dgvDetalles.Rows.Count == 0)
        { MessageBox.Show("Debe agregar al menos un articulo!"); return; }

        if (servicio.AgregarOrden(NewOrder))
        { MessageBox.Show("Agregado con exito!"); }
        else { MessageBox.Show("No agregado"); }


        Limpiar();
        //completar...
    }

    private void Limpiar()
    {

        dgvDetalles.Rows.Clear();
        txtResp.Text = string.Empty;
        dtpFecha.Text = DateTime.Now.ToString();
        nudCantidad.Value = 1;
        cboMateriales.SelectedIndex = 0;


    }
    private void btnCancelar_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Desea cancelar?", "Cancelar", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)

        {
            dgvDetalles.Rows.Clear();
            txtResp.Text = string.Empty;
            dtpFecha.Text = DateTime.Now.ToString();
            nudCantidad.Value = 1;
            cboMateriales.SelectedIndex = 0;

        }
    }

    private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e)
    {
        CargarCombo();
    }

    private void CargarCombo()
    {

        cboMateriales.DataSource = servicio.TraerMateriales();
        cboMateriales.DisplayMember = "nombre";
        cboMateriales.ValueMember = "codigo";
        cboMateriales.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    private void btnAgregar_Click(object sender, EventArgs e)
    {
        Materiales materi = (Materiales)cboMateriales.SelectedItem;
        // trae el elemento que esta elegido en el combobox y hace un casteo (tengo todos sus atributos)
        //DataRowView item = (DataRowView)cboMateriales.SelectedItem;



        if (ExisteProductoEnGrilla(materi.Nombre.ToString()))
        {
            MessageBox.Show("El producto ya fue agregado");
            return;

        }

        int stock = Convert.ToInt32(materi.Stock);

        if (stock < nudCantidad.Value)
        {
            MessageBox.Show("No hay stock suficiente");
            return;

        }

        int cod_material = Convert.ToInt32(materi.Codigo);
        string nombre_material = materi.Nombre;
        int cantidad = Convert.ToInt32(nudCantidad.Value);
        string responsable = txtResp.Text;

        dgvDetalles.Rows.Add(cod_material, nombre_material, stock, cantidad);


        DetalleOrden det = new DetalleOrden(materi, cantidad);

        NewOrder.AgregarDetalle(det);
        NewOrder.Responsable = responsable;


    }

    private bool ExisteProductoEnGrilla(string text)
    {
        foreach (DataGridViewRow fila in dgvDetalles.Rows)
        {
            if (fila.Cells["material"].Value == text)
                return true;

        }

        return false;
    }


    private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        //boton quitar
        if (dgvDetalles.CurrentCell.ColumnIndex == 4)
        {
            NewOrder.QuitarDetalle(dgvDetalles.CurrentRow.Index);
            dgvDetalles.Rows.RemoveAt(dgvDetalles.CurrentRow.Index);

        }
    }
}



//Acceso a datos/HelperDAo

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


//Conexion dao

internal class ConexionDao
{
    private SqlConnection cnn = new SqlConnection(Properties.Resources.StringSql);
    private SqlCommand cmd;
    private SqlTransaction trans;


    public void Conectar()
    {
        cnn.Open();
        cmd = new SqlCommand();
        cmd.Connection = cnn;
    }

    public void Desconectar()
    {
        cnn.Close();
    }

    public DataTable Consultar()
    {
        Conectar(); // se conecta y crea comando
        cmd.CommandText = "sp_consultar_materiales"; //paso nombre procedimiento almacenado
        cmd.CommandType = CommandType.StoredProcedure; //paso que voy a ejecutar un sp
        DataTable dt = new DataTable();  //creo una datatable
        dt.Load(cmd.ExecuteReader());//cargo en la datatable la consulta
        Desconectar(); //desconecto
        return dt;  //devuelvo datatable
    }


    public bool AgregarOrden(OrdenRetiro orden)
    {

        bool resultado = false;
        trans = null;
        try
        {
            Conectar();
            trans = cnn.BeginTransaction();
            SqlCommand cmd = new SqlCommand("SP_INSERTAR_ORDEN", cnn, trans);
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@responsable", orden.Responsable);

            SqlParameter p1 = new SqlParameter();
            p1.ParameterName = "@nro";
            p1.Direction = ParameterDirection.Output;
            p1.SqlDbType = SqlDbType.Int;
            cmd.Parameters.Add(p1);

            cmd.ExecuteNonQuery();
            int nro = Convert.ToInt32(p1.Value);

            int detalle_nro = 1;
            SqlCommand cmd1;

            foreach (DetalleOrden det in orden.DetOrd)
            {
                cmd1 = new SqlCommand("SP_INSERTAR_DETALLES", cnn, trans);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@nro_orden", nro);
                cmd1.Parameters.AddWithValue("@detalle", detalle_nro);
                cmd1.Parameters.AddWithValue("@codigo", det.Material.Codigo);
                cmd1.Parameters.AddWithValue("@cantidad", det.Cantidad);
                cmd1.ExecuteNonQuery();
                detalle_nro++;
            }


            trans.Commit();
            resultado = true;
        }

        catch (SqlException)
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
            Desconectar();
        }

        return resultado;

    }

}


//Acceso a datos/interdaz/IOrdenDAo
public interface IOrdenDao
{

    bool AgregarOrden(OrdenRetiro orden);
    List<Materiales> TraerMateriales();

}


//Acceso a datos/implementacion/OrdenDAo
internal class OrdenRetiroDao : IOrdenDao
{

    public bool AgregarOrden(OrdenRetiro orden)
    {
        SqlTransaction trans;
        SqlConnection cnn = HelperDao.ObtenerInstancia().ObtenerConexion();
        bool resultado = false;
        trans = null;
        try
        {
            cnn.Open();

            trans = cnn.BeginTransaction();
            SqlCommand cmd = new SqlCommand("SP_INSERTAR_ORDEN", cnn, trans);
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@responsable", orden.Responsable);

            SqlParameter p1 = new SqlParameter();
            p1.ParameterName = "@nro";
            p1.Direction = ParameterDirection.Output;
            p1.SqlDbType = SqlDbType.Int;
            cmd.Parameters.Add(p1);

            cmd.ExecuteNonQuery();
            int nro = Convert.ToInt32(p1.Value);

            int detalle_nro = 1;
            SqlCommand cmd1;

            foreach (DetalleOrden det in orden.DetOrd)
            {
                cmd1 = new SqlCommand("SP_INSERTAR_DETALLES", cnn, trans);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@nro_orden", nro);
                cmd1.Parameters.AddWithValue("@detalle", detalle_nro);
                cmd1.Parameters.AddWithValue("@codigo", det.Material.Codigo);
                cmd1.Parameters.AddWithValue("@cantidad", det.Cantidad);
                cmd1.ExecuteNonQuery();
                detalle_nro++;
            }


            trans.Commit();
            resultado = true;
        }

        catch (SqlException)
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



    public List<Materiales> TraerMateriales()
    {
        SqlConnection cnn = HelperDao.ObtenerInstancia().ObtenerConexion();
        cnn.Open();
        SqlCommand cmd = new SqlCommand();// se conecta y crea comando
        cmd.Connection = cnn;
        cmd.CommandText = "sp_consultar_materiales"; //paso nombre procedimiento almacenado
        cmd.CommandType = CommandType.StoredProcedure; //paso que voy a ejecutar un sp
        List<Materiales> lst = new List<Materiales>();
        DataTable dt = new DataTable();//creo una datatable
        dt.Load(cmd.ExecuteReader());//cargo en la datatable la consulta
        cnn.Close(); //desconecto

        foreach (DataRow dr in dt.Rows)
        {
            int codigo = Convert.ToInt32(dr[0].ToString());
            string nombre = dr[1].ToString();
            double stock = Convert.ToDouble(dr["stock"].ToString());
            Materiales m = new Materiales(codigo, nombre, stock);
            lst.Add(m);


        }


        return lst;  //devuelvo datatable
    }
}


//Consultar-borrar

public partial class FrmConsulta : Form
{
    IServicio servicio;
    public FrmConsulta()
    {
        InitializeComponent();
        servicio = new Servicio();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        dtpDesde.Value = DateTime.Now.AddDays(-30);
        CargarClientes();
    }

    private void CargarClientes()
    {
        cboClientes.ValueMember = "id_cliente";
        cboClientes.DisplayMember = "nombre";
        cboClientes.DataSource = servicio.TraerClientes();
        cboClientes.SelectedIndex = -1;
        cboClientes.DropDownStyle = ComboBoxStyle.DropDownList;

    }

    private void groupBox2_Enter(object sender, EventArgs e)
    {

    }

    private void btnConsultar_Click(object sender, EventArgs e)
    {
        List<Parametro> paramList = new List<Parametro>();
        int error_de_botta = cboClientes.SelectedIndex + 1;
        paramList.Add(new Parametro("@cliente", error_de_botta));
        paramList.Add(new Parametro("@fecha_desde", dtpDesde.Value));
        paramList.Add(new Parametro("@fecha_hasta", dtpHasta.Value));
        DataTable dt = new DataTable();
        dt = HelperDao.getInstance().Consultar("SP_CONSULTAR_PEDIDOS", paramList);

        dgvPedidos.Rows.Clear();
        foreach (DataRow f in dt.Rows)
        {
            dgvPedidos.Rows.Add(f[0].ToString(), f[6].ToString(), f[5].ToString(), f[4].ToString());


        }

    }

    private bool Validar()
    {
        if (cboClientes.SelectedIndex == -1)
        {
            MessageBox.Show("Debe Ingresar un Cliente", "Seleccionar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        if (true)
        {

        }
        return true;
    }

    private void dgvPedidos_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (dgvPedidos.CurrentCell.ColumnIndex == 4)
        {
            if ((string)dgvPedidos.CurrentRow.Cells[3].Value == "N")
            { MessageBox.Show("Ya esta entragado como tu mujer!"); return; }
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@codigo";
            p.SqlDbType = SqlDbType.Int;
            p.Direction = ParameterDirection.Input;
            p.Value = dgvPedidos.CurrentRow.Cells[0].Value;
            HelperDao.getInstance().Executer("SP_REGISTRAR_ENTREGA", p);
            MessageBox.Show("Entrega realizada");

        }
        if (dgvPedidos.CurrentCell.ColumnIndex == 5)
        {
            if (MessageBox.Show("Desea eliminar?", "Eliminar", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                SqlParameter p = new SqlParameter();
                p.ParameterName = "@codigo";
                p.SqlDbType = SqlDbType.Int;
                p.Direction = ParameterDirection.Input;
                p.Value = dgvPedidos.CurrentRow.Cells[0].Value;
                HelperDao.getInstance().Executer("SP_REGISTRAR_BAJA", p);
                MessageBox.Show("Baja realizada");
            }

        }
    }
}
