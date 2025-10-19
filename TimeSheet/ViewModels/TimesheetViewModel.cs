// TimesheetViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace TimeSheet.ViewModels
{
    public partial class TimesheetViewModel : ObservableObject
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IEmployeeService _employeeService;

        public bool HasTimesheets => Timesheets?.Any() == true;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private Employee _selectedEmployee;

        [ObservableProperty]
        private ObservableCollection<Timesheet> _timesheets;

        [ObservableProperty]
        private int _selectedMonth = DateTime.Now.Month;

        [ObservableProperty]
        private int _selectedYear = DateTime.Now.Year;

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

        partial void OnSelectedEmployeeChanged(Employee value)
        {
            _ = LoadTimesheetsAsync();
        }
    }
}