using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.UI.Services;
using TimeBasedAccounting.UI.ViewModels;
using TimeBasedAccounting.UI.Views;

namespace TimeBasedAccounting.UI
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            // === Загружаем appsettings.json ===
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(config);

            // === Регистрируем DbContext для MySQL ===
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<AccountingDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // === Регистрируем сервисы, ViewModel и Views ===
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<NavigationService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();

            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            // === Стартуем приложение ===
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var loginView = _serviceProvider.GetRequiredService<LoginView>();
                loginView.DataContext = _serviceProvider.GetRequiredService<LoginViewModel>();

                desktop.MainWindow = loginView;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}