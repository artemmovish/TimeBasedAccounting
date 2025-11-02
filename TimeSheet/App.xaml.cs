using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Services;
using TimeSheet.ViewModels;
using TimeSheet.Windows;

namespace TimeSheet
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Инициализация базы данных и заполнение тестовыми данными
            InitializeDatabase();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Создаем окно входа
            var loginWindow = new LoginWindow();

            // Устанавливаем DataContext для окна входа
            loginWindow.DataContext = ServiceProvider.GetRequiredService<LoginViewModel>();

            if (loginWindow.ShowDialog() == true)
            {
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                // Если вход успешен, показываем главное окно
                var mainWindow = new MainWindow();
                mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Доступ запрещен");
                Shutdown();
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Конфигурация
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Регистрация DbContext
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AccountingDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Регистрация сервисов
            services.AddScoped<ITimesheetService, TimesheetService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IVacationService, VacationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();

            // Регистрация ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<TimesheetViewModel>();
            services.AddTransient<EmployeesViewModel>();
            services.AddTransient<VacationsViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<LoginViewModel>();
            // Регистрация ViewModels для операций
            services.AddTransient<EmployeeOperationViewModel>();
            services.AddTransient<TimesheetOperationViewModel>();
            services.AddTransient<LatenessOperationViewModel>();
            services.AddTransient<CreateUserViewModel>();
        }

        private void InitializeDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();

            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Заполнение тестовыми данными
            DbSeeder.Seed(db);
        }
    }
}