// VacationsViewModel.cs (обновленная версия)
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TimeBasedAccounting.Core;
using System.Windows;

namespace TimeSheet.ViewModels
{
    public partial class VacationsViewModel : ObservableObject
    {
        private readonly IVacationService _vacationService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private bool _isAdmin = ActiveUser.RoleId ? true : false;

        [ObservableProperty]
        private ObservableCollection<Vacation> _vacations;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private Employee _selectedEmployee;

        [ObservableProperty]
        private object _currentOperationView;

        [ObservableProperty]
        private bool _isOperationMode;

        public VacationsViewModel(IVacationService vacationService, IEmployeeService employeeService)
        {
            _vacationService = vacationService;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);

            await LoadVacationsAsync();
        }

        [RelayCommand]
        private async Task LoadVacationsAsync()
        {
            var vacations = await _vacationService.GetVacationsAsync(
                SelectedEmployee?.EmployeeId);
            Vacations = new ObservableCollection<Vacation>(vacations);
        }

        [RelayCommand]
        private void CreateVacation()
        {
            var vacationOperationVM = new VacationOperationViewModel(_vacationService, _employeeService);
            vacationOperationVM.OnOperationCompleted += OnVacationOperationCompleted;

            CurrentOperationView = new Views.VacationOperationView() { DataContext = vacationOperationVM };
            IsOperationMode = true;
        }

        private void OnVacationOperationCompleted(object sender, EventArgs e)
        {
            IsOperationMode = false;
            CurrentOperationView = null;

            // Обновляем список отпусков после операции
            _ = LoadVacationsAsync();
        }

        partial void OnSelectedEmployeeChanged(Employee value)
        {
            _ = LoadVacationsAsync();
        }
    }
}