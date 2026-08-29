using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Mappers;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.Cliente;
using TP_ControlVehicular.Presentacion.Marca;
using TP_ControlVehicular.Presentacion.Modelo;
using TP_ControlVehicular.Presentacion.Vehiculo;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var services = new ServiceCollection();
            // Registrar logging para que AutoMapper y otros componentes que lo requieran
            // puedan resolverse desde el contenedor de dependencias.
            services.AddLogging();
            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            // DbContext
            var builder = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            services.AddDbContext<CVDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            // Repositorios
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IVehiculoRepository, VehiculoRepository>();
            services.AddScoped<IModeloRepository, ModeloRepository>();
            services.AddScoped<IMarcaRepository, MarcaRepository>();

            // Handlers
            services.AddScoped<RegistrarClienteHandler>();
            services.AddScoped<RegistrarVehiculoHandler>();
            services.AddScoped<ListarVehiculosPorClienteHandler>();


            // ViewModels
            services.AddScoped<ClienteViewModel>();
            services.AddScoped<VehiculoViewModel>();
            services.AddScoped<ModeloViewModel>();
            services.AddScoped<MarcaViewModel>();

            // UserControls
            services.AddScoped<CtlCliente>();
            services.AddScoped<CtlVehiculo>();
            services.AddScoped<CtlModelo>();
            services.AddScoped<CtlMarca>();

            // Ventana principal
            services.AddSingleton<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }

}
