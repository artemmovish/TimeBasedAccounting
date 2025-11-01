using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeSheet.ViewModels
{
    public partial class TimesheetOperationViewModel : ObservableObject
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private Timesheet _timesheet = new Timesheet { Date = DateTime.Now };

        [ObservableProperty]
        private string _title = "Добавление записи табеля";

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private ObservableCollection<AttendanceMarker> _attendanceMarkers;

        public TimesheetOperationViewModel(ITimesheetService timesheetService, IEmployeeService employeeService)
        {
            _timesheetService = timesheetService;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);

            // Загрузка маркеров посещаемости (в реальном приложении нужен соответствующий сервис)
            AttendanceMarkers = new ObservableCollection<AttendanceMarker>
            {
                new AttendanceMarker { MarkerId = 1, Code = "Я", Description = "Явка" },
                new AttendanceMarker { MarkerId = 2, Code = "ОТП", Description = "Отпуск" },
                new AttendanceMarker { MarkerId = 3, Code = "Б", Description = "Больничный" },
                new AttendanceMarker { MarkerId = 4, Code = "Н", Description = "Неявка" },
                new AttendanceMarker { MarkerId = 5, Code = "Я/О", Description = "Явка с опозданием" },
                new AttendanceMarker { MarkerId = 6, Code = "К", Description = "Командировка" },
                new AttendanceMarker { MarkerId = 7, Code = "В", Description = "Выходной" }
            };

            //// Установка значений по умолчанию
            //if (Employees.Any())
            //    Timesheet.EmployeeId = Employees.First().EmployeeId;

            //if (AttendanceMarkers.Any())
            //    Timesheet.MarkerId = AttendanceMarkers.First().MarkerId;
        }

        public void SetTimesheetForEdit(Timesheet timesheet)
        {
            if (timesheet != null)
            {
                Timesheet = timesheet;
                Title = "Редактирование записи табеля";
            }
        }

        [RelayCommand]
        private async Task SaveTimesheetAsync()
        {
            try
            {
                if (Timesheet.EmployeeId == 0 || Timesheet.MarkerId == 0)
                {
                    System.Windows.MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                var result = await _timesheetService.AddOrUpdateTimesheetAsync(Timesheet);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при сохранении записи: {ex.Message}");
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