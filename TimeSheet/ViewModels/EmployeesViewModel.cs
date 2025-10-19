// EmployeesViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        private bool _onlyActive = true;

        public EmployeesViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var departments = await _employeeService.GetDepartmentsAsync();
            Departments = new ObservableCollection<Department>(departments);

            await LoadEmployeesAsync();
        }

        [RelayCommand]
        private async Task LoadEmployeesAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync(
                SelectedDepartment?.DepartmentId, OnlyActive);
            Employees = new ObservableCollection<Employee>(employees);
        }

        partial void OnSelectedDepartmentChanged(Department value)
        {
            _ = LoadEmployeesAsync();
        }

        partial void OnOnlyActiveChanged(bool value)
        {
            _ = LoadEmployeesAsync();
        }
    }
}