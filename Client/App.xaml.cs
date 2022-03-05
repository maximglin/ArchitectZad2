using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GrpcClient;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider = CreateServiceProvider();

            Window w = new MainWindow();
            w.DataContext = ServiceProvider.GetRequiredService<CarsVM>();
            w.Show();

            base.OnStartup(e);
        }

        IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IRepository<CarReply, CarUpdateRequest>, CarsRepository>();
            services.AddSingleton<IRepository<ManufacturerMessage, ManufacturerMessage>, ManufacturersRepository>();
            services.AddSingleton<IRepository<ColorMessage, ColorMessage>, ColorsRepository>();


            services.AddScoped<CarsVM>();

            return services.BuildServiceProvider();
        }
    }
}
