using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core.Services;

namespace TimeSheet.ViewModels
{
    public partial class EmployeeOperationViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private Employee _employee = new Employee();

        [ObservableProperty]
        private string _title = "Добавление сотрудника";

        [ObservableProperty]
        private ObservableCollection<Department> _departments;


        public EmployeeOperationViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _ = LoadDepartmentsAsync();
        }


        private async Task LoadDepartmentsAsync()
        {
            var departments = await _employeeService.GetDepartmentsAsync();
            Departments = new ObservableCollection<Department>(departments);

            // Установка отдела по умолчанию
            if (Departments.Any() && Employee.DepartmentId == 0)
                Employee.DepartmentId = Departments.First().DepartmentId;
        }

        public void SetEmployeeForEdit(Employee employee)
        {
            if (employee != null)
            {
                Employee = employee;
                Title = "Редактирование сотрудника";
            }
        }

        [RelayCommand]
        private async Task SaveEmployeeAsync()
        {
            try
            {
                await _employeeService.CreateEmployeeAsync(Employee);

                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnOperationCompleted;
    }
}