// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TimeBasedAccounting.Core;


namespace TimeSheet.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private object _currentViewModel;

        [ObservableProperty]
        private string _currentUser = ActiveUser.RoleId ? $"{ActiveUser.FullName} - Табельщик" : $"{ActiveUser.FullName} - Админ";

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            CurrentViewModel = _serviceProvider.GetRequiredService<TimesheetViewModel>();
        }

        [RelayCommand]
        private void Navigate(string page)
        {
            CurrentViewModel = page switch
            {
                "timesheet" => _serviceProvider.GetRequiredService<TimesheetViewModel>(),
                "employees" => _serviceProvider.GetRequiredService<EmployeesViewModel>(),
                "vacations" => _serviceProvider.GetRequiredService<VacationsViewModel>(),
                "reports" => _serviceProvider.GetRequiredService<ReportsViewModel>(),
                _ => CurrentViewModel
            };
        }
    }
}