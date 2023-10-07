
using ParcialApp.Entidades;
using ParcialApp.Servicios.Implementacion;
using ParcialApp.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParcialApp.Presentacion
{
    public partial class Frm_Alta : Form
    {

        Conexion helper;
        OrdenRetiro nueva;
        IServicio servicio = null;
        

        public Frm_Alta()
        {
            InitializeComponent();
            helper = new Conexion();
            nueva = new OrdenRetiro();
            servicio = new Servicio();

        }



            private void btnAceptar_Click(object sender, EventArgs e)
            {
            if (string.IsNullOrEmpty(txtResp.Text))
            {
                MessageBox.Show("Agregue un responsable");
                return;
            }
            if (dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Compra algo rata");
                return;
            }

            if (servicio.GrabarOrden(nueva))
            {
                MessageBox.Show("Agregado con exito!");

            }
            else 
            { 
                MessageBox.Show("No se pudo agregar");
            }
            Limpiar();
          
        }

        private void Limpiar()
        {
            nudCantidad.Value = 1;
            txtResp.Text = string.Empty;
            dtpFecha.Text = DateTime.Now.ToString();
            dgvDetalles.Rows.Clear();
            cboMateriales.SelectedIndex = 0;
        


        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Desea cancelar?", "Cancelar", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            { Limpiar(); }
          
        }

        private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e)
        {
            CargarCombo();
            dtpFecha.Enabled = false;
        }

        private void CargarCombo()
        {
            List<Materiales> lstm = new List<Materiales>();
            lstm = servicio.Consultar();
            cboMateriales.DataSource = lstm;
            cboMateriales.ValueMember = "codigo";
            cboMateriales.DisplayMember = "nombre";
            cboMateriales.DropDownStyle = ComboBoxStyle.DropDownList;
          
          
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
           

            Materiales mat = (Materiales)cboMateriales.SelectedItem;

            foreach (DataGridViewRow fila in dgvDetalles.Rows)
            {
                if (mat.Nombre == fila.Cells["material"].Value.ToString())
                { 
                    MessageBox.Show("Ya esta agregado"); 
                    return; 
                }
            }







            if (Convert.ToInt32(mat.Stock) < nudCantidad.Value)
            {
                MessageBox.Show("Cantidad insuficiente!!!");
                return;
            }

            

            dgvDetalles.Rows.Add(mat.Codigo, mat.Nombre, mat.Stock, nudCantidad.Value);
            int cantidad = Convert.ToInt32(nudCantidad.Value);

            DetalleOrden det = new DetalleOrden(mat, cantidad);
            nueva.AgregarDetalle(det);
            nueva.Responsable = txtResp.Text;






        }

        private bool ExisteProductoEnGrilla(string text)
        {
            return false;
        }


        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvDetalles.CurrentCell.ColumnIndex == 4)
            {
                nueva.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                dgvDetalles.Rows.RemoveAt(dgvDetalles.CurrentRow.Index);
            
            
            }
            //boton quitar
          
        }
    }
}
