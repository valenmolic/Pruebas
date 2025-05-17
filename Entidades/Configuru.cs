using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    internal class Configuru
    {
        ///FECHA
        private void ConfigurarControles()
        {
            // Configurar DateTimePicker
            dateFecha.MinDate = new DateTime(2000, 1, 1);
            dateFecha.MaxDate = new DateTime(2050, 12, 31);

            // Configurar otros controles...
        }

        // Llamar desde el constructor o Load
        public FrmConsultas()
        {
            InitializeComponent();
            ConfigurarControles();
            consultasService = new ConsultaVeterinariaService();
        }

        // Otras fechas
        private void LimpiarCampos()
        {
            txtId.Clear();

            // PRIMERO: Configurar el rango de fechas permitido
            dateFecha.MinDate = new DateTime(2000, 1, 1);
            dateFecha.MaxDate = new DateTime(2050, 12, 31);

            // DESPUÉS: Asignar el valor de la fecha actual
            DateTime fechaActual = DateTime.Now;
            dateFecha.Value = fechaActual; // Ahora esto funcionará porque la fecha está dentro del rango

            txtTratamiento.Clear();
            txtDiagnostico.Clear();
            txtBuscarId.Clear();
            txtBuscarMascota.Clear();
            txtId.Enabled = true;
            CargarLista();
            btnLimpiar.Enabled = false;
            btnGuardar.Enabled = true;
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
        }

        //otros
        private void BuscarConsulta(TextBox txtBuscarId)
        {
            if (string.IsNullOrEmpty(txtBuscarId.Text))
            {
                MessageBox.Show("Por favor ingrese el ID de la consulta a buscar");
                return;
            }
            if (!int.TryParse(txtBuscarId.Text, out int id))
            {
                MessageBox.Show("El ID debe ser un numero entero");
                return;
            }
            var consulta = consultasService.GetById(id);
            if (consulta != null)
            {
                CargarLista(consulta);
                btnLimpiar.Enabled = true;

                // Si estás mostrando los detalles de la consulta aquí, verifica la fecha
                if (consulta.Fecha < dateFecha.MinDate || consulta.Fecha > dateFecha.MaxDate)
                {
                    MessageBox.Show($"La fecha de la consulta ({consulta.Fecha.ToShortDateString()}) está fuera del rango permitido.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Consulta no encontrada");
            }
        }
    }
}
