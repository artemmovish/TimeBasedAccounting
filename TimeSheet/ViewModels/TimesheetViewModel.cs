using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeSheet.ViewModels
{
    public partial class TimesheetViewModel : ObservableObject
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private Employee _selectedEmployee;

        [ObservableProperty]
        private ObservableCollection<Timesheet> _timesheets;

        [ObservableProperty]
        private Timesheet _selectedTimesheet;

        [ObservableProperty]
        private int _selectedMonth = DateTime.Now.Month;

        [ObservableProperty]
        private int _selectedYear = DateTime.Now.Year;

        [ObservableProperty]
        private object _currentOperationView;

        [ObservableProperty]
        private bool _isOperationMode;

        public bool HasTimesheets => Timesheets?.Any() == true;

        public TimesheetViewModel(ITimesheetService timesheetService, IEmployeeService employeeService)
        {
            _timesheetService = timesheetService;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);
            SelectedEmployee = Employees.FirstOrDefault();
        }

        [RelayCommand]
        private async Task LoadTimesheetsAsync()
        {
            if (SelectedEmployee != null)
            {
                var timesheets = await _timesheetService.GetTimesheetsAsync(
                    SelectedEmployee.EmployeeId, SelectedMonth, SelectedYear);
                Timesheets = new ObservableCollection<Timesheet>(timesheets);
            }
        }

        [RelayCommand]
        private void CreateTimesheet()
        {
            var vm = App.ServiceProvider.GetRequiredService<TimesheetOperationViewModel>();
            vm.OnOperationCompleted += OnTimesheetOperationCompleted;
            CurrentOperationView = new Views.TimesheetOperationView { DataContext = vm };
            IsOperationMode = true;
        }

        [RelayCommand]
        private void EditTimesheet()
        {
            if (SelectedTimesheet == null) return;

            var vm = App.ServiceProvider.GetRequiredService<TimesheetOperationViewModel>();
            vm.SetTimesheetForEdit(SelectedTimesheet);
            vm.OnOperationCompleted += OnTimesheetOperationCompleted;
            CurrentOperationView = new Views.TimesheetOperationView { DataContext = vm };
            IsOperationMode = true;
        }

        [RelayCommand]
        private void AddLateness()
        {
            if (SelectedTimesheet == null) return;

            var vm = App.ServiceProvider.GetRequiredService<LatenessOperationViewModel>();
            vm.SetTimesheetId(SelectedTimesheet.TimesheetId);
            vm.OnOperationCompleted += OnLatenessOperationCompleted;
            CurrentOperationView = new Views.LatenessOperationView { DataContext = vm };
            IsOperationMode = true;
        }

        private void OnTimesheetOperationCompleted(object sender, EventArgs e)
        {
            if (sender is TimesheetOperationViewModel vm)
            {
                vm.OnOperationCompleted -= OnTimesheetOperationCompleted;
            }

            IsOperationMode = false;
            CurrentOperationView = null;
            _ = LoadTimesheetsAsync(); // Обновляем данные
        }

        private void OnLatenessOperationCompleted(object sender, EventArgs e)
        {
            if (sender is LatenessOperationViewModel vm)
            {
                vm.OnOperationCompleted -= OnLatenessOperationCompleted;
            }

            IsOperationMode = false;
            CurrentOperationView = null;
            _ = LoadTimesheetsAsync(); // Обновляем данные
        }

        partial void OnSelectedEmployeeChanged(Employee value)
        {
            _ = LoadTimesheetsAsync();
        }
    }
}