using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core.Interfaces;

namespace TimeSheet.ViewModels
{
    public partial class EmployeeOperationViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private Employee _employee = new Employee();

        [ObservableProperty]
        private string _title = "Добавление сотрудника";

        public EmployeeOperationViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
                // Здесь будет логика сохранения сотрудника
                // В реальном приложении нужно добавить соответствующие методы в сервис

                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
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