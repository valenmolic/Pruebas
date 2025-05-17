using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.VetVida.BLL;
using Entidades.VetVida.DAL;
using Entidades.VetVida.Ents;

namespace Entidades
{
    internal class OtraSolii
    {
        // ENTIDAD Raza.cs
namespace VetVida.Ents
    {
        public class Raza : BaseEntity
        {
            public string Nombre { get; set; }
            public int IdEspecie { get; set; }

            public override string ToString()
            {
                return $"{Id};{Nombre};{IdEspecie}";
            }
        }
    }

// REPOSITORIO RazaRepository.cs
using VetVida.Ents;

namespace VetVida.DAL
    {
        public class RazaRepository : FileRepository<Raza>
        {
            public RazaRepository(string ruta) : base(ruta) { }

            public override Raza Mappear(string datos)
            {
                string[] campos = datos.Split(';');
                return new Raza
                {
                    Id = int.Parse(campos[0]),
                    Nombre = campos[1],
                    IdEspecie = int.Parse(campos[2])
                };
            }
        }
    }

// SERVICIO RazaService.cs
using VetVida.DAL;
using VetVida.Ents;

namespace VetVida.BLL
    {
        public class RazaService : IService<Raza>
        {
            private readonly RazaRepository repo = new RazaRepository(Archivos.ARC_RAZA);

            public string Guardar(Raza entity)
            {
                if (string.IsNullOrWhiteSpace(entity.Nombre))
                    return "El nombre es obligatorio";

                if (Consultar().Any(r => r.Nombre.ToLower() == entity.Nombre.ToLower()))
                    return "Ya existe una raza con ese nombre";

                return repo.Guardar(entity);
            }

            public List<Raza> Consultar() => repo.Consultar();

            public string Modificar(Raza entity) => repo.Modificar(entity);

            public string Eliminar(int id) => repo.Eliminar(id);
        }
    }

    // FORMULARIO (UI) - métodos clave
    private RazaService serviceRaza = new RazaService();

    private void btnGuardar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("Debe ingresar el nombre de la raza.");
            return;
        }

        var raza = new Raza
        {
            Id = int.Parse(txtId.Text),
            Nombre = txtNombre.Text.Trim(),
            IdEspecie = ((Especie)cmbEspecie.SelectedItem).Id
        };

        var mensaje = serviceRaza.Guardar(raza);
        MessageBox.Show(mensaje);
        if (mensaje.ToLower().Contains("exitosamente") || mensaje.ToLower().Contains("correctamente"))
        {
            CargarRazas();
            LimpiarCampos();
        }
    }

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (lstRazas.SelectedItem is Raza seleccionada)
        {
            var confirm = MessageBox.Show("¿Eliminar raza?", "Confirmación", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                var mensaje = serviceRaza.Eliminar(seleccionada.Id);
                MessageBox.Show(mensaje);
                CargarRazas();
            }
        }
    }

    private void CargarRazas()
    {
        lstRazas.DataSource = serviceRaza.Consultar();
        lstRazas.DisplayMember = "Nombre";
    }

    private void LimpiarCampos()
    {
        txtId.Clear();
        txtNombre.Clear();
        cmbEspecie.SelectedIndex = -1;
    }

}
}
