// VacationOperationViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core;

namespace TimeSheet.ViewModels
{
    public partial class VacationOperationViewModel : ObservableObject
    {
        private readonly IVacationService _vacationService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private Vacation _vacation = new Vacation();

        [ObservableProperty]
        private string _title = "Добавление отпуска";

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<VacationType> _vacationTypes;

        [ObservableProperty]
        private ObservableCollection<VacationStatus> _vacationStatuses;

        public VacationOperationViewModel(IVacationService vacationService, IEmployeeService employeeService)
        {
            _vacationService = vacationService;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            // Загрузка сотрудников
            var employees = await _employeeService.GetEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);

            // Загрузка типов отпусков
            var vacationTypes = await _vacationService.GetVacationTypesAsync();
            VacationTypes = new ObservableCollection<VacationType>(vacationTypes);

            // Загрузка статусов отпусков
            var vacationStatuses = await _vacationService.GetVacationStatusesAsync();
            VacationStatuses = new ObservableCollection<VacationStatus>(vacationStatuses);

            // Установка значений по умолчанию
            if (Employees.Any() && Vacation.EmployeeId == 0)
                Vacation.EmployeeId = Employees.First().EmployeeId;

            if (VacationTypes.Any() && Vacation.VacationTypeId == 0)
                Vacation.VacationTypeId = VacationTypes.First().TypeId;

            if (VacationStatuses.Any() && Vacation.VacationStatusId == 0)
                Vacation.VacationStatusId = VacationStatuses.First().StatusId;

            // Установка текущего пользователя как создателя
            Vacation.CreatedBy = ActiveUser.UserId;
        }

        public void SetVacationForEdit(Vacation vacation)
        {
            if (vacation != null)
            {
                Vacation = vacation;
                Title = "Редактирование отпуска";
            }
        }

        [RelayCommand]
        private async Task SaveVacationAsync()
        {
            try
            {
                // Валидация
                if (Vacation.StartDate >= Vacation.EndDate)
                {
                    MessageBox.Show("Дата начала отпуска должна быть раньше даты окончания", "Ошибка");
                    return;
                }
                
                if (Vacation.EmployeeId == 0)
                {
                    MessageBox.Show("Выберите сотрудника", "Ошибка");
                    return;
                }

                // Сохранение
                var savedVacation = await _vacationService.CreateVacationAsync(Vacation);

                MessageBox.Show("Отпуск успешно сохранен", "Успех");
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отпуска: {ex.Message}", "Ошибка");
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