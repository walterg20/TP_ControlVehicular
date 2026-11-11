// walterg20
using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class FrmSetupInicial : Window
    {
        public FrmSetupInicial()
        {
            InitializeComponent();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password != pbPassword2.Password || string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Las contraseñas no coinciden o están vacías.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtEmpresa.Text))
            {
                MessageBox.Show("Ingrese el nombre de la empresa.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var factory = App.ServiceProvider!.GetRequiredService<IDbContextFactory<CVDbContext>>();
                using var ctx = factory.CreateDbContext();

                var admin = ctx.Usuarios.FirstOrDefault(u => u.Dni == "10000001");
                if (admin != null)
                {
                    admin.Contrasena = pbPassword.Password;
                    admin.Activo = true;
                }

                var empresa = ctx.ConfiguracionEmpresa.FirstOrDefault();
                if (empresa == null)
                {
                    empresa = new ConfiguracionEmpresa { Id = 1 };
                    ctx.ConfiguracionEmpresa.Add(empresa);
                }
                empresa.Nombre = txtEmpresa.Text.Trim();
                empresa.Direccion = txtDireccion.Text.Trim();
                empresa.Telefono = txtTelefono.Text.Trim();
                empresa.FechaActualizacion = DateTime.Now;

                ctx.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la configuración inicial: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}