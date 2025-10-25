using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeSheet.ViewModels
{
    public partial class EmployeesViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<Department> _departments;

        [ObservableProperty]
        private Department _selectedDepartment;

        [ObservableProperty]
        private Employee _selectedEmployee;

        [ObservableProperty]
        private bool _onlyActive = true;

        [ObservableProperty]
        private object _currentOperationView;

        [ObservableProperty]
        private bool _isOperationMode;

        public EmployeesViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadDepartmentsAsync();
            await LoadEmployeesAsync();
        }

        [RelayCommand]
        private async Task LoadEmployeesAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync(
                SelectedDepartment?.DepartmentId, OnlyActive);
            Employees = new ObservableCollection<Employee>(employees);
        }

        private async Task LoadDepartmentsAsync()
        {
            var departments = await _employeeService.GetDepartmentsAsync();
            Departments = new ObservableCollection<Department>(departments);
        }

        [RelayCommand]
        private void CreateEmployee()
        {
            var vm = App.ServiceProvider.GetRequiredService<EmployeeOperationViewModel>();
            vm.OnOperationCompleted += OnEmployeeOperationCompleted;
            CurrentOperationView = new Views.EmployeeOperationView { DataContext = vm };
            IsOperationMode = true;
        }

        [RelayCommand]
        private void EditEmployee()
        {
            if (SelectedEmployee == null) return;

            var vm = App.ServiceProvider.GetRequiredService<EmployeeOperationViewModel>();
            vm.SetEmployeeForEdit(SelectedEmployee);
            vm.OnOperationCompleted += OnEmployeeOperationCompleted;
            CurrentOperationView = new Views.EmployeeOperationView { DataContext = vm };
            IsOperationMode = true;
        }

        [RelayCommand]
        private async Task DeactivateEmployeeAsync()
        {
            if (SelectedEmployee == null) return;

            // Логика деактивации сотрудника
            await LoadEmployeesAsync();
        }

        private void OnEmployeeOperationCompleted(object sender, EventArgs e)
        {
            if (sender is EmployeeOperationViewModel vm)
            {
                vm.OnOperationCompleted -= OnEmployeeOperationCompleted;
            }

            IsOperationMode = false;
            CurrentOperationView = null;
            _ = LoadEmployeesAsync(); // Обновляем список
        }

        partial void OnSelectedDepartmentChanged(Department value)
        {
            _ = LoadEmployeesAsync();
        }
    }
}