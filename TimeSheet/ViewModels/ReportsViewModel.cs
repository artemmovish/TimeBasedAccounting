// ReportsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace TimeSheet.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly IReportService _reportService;
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private ObservableCollection<Department> _departments;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private Department _selectedDepartment;

        [ObservableProperty]
        private Employee _selectedEmployee;

        [ObservableProperty]
        private DateTime _selectedPeriod = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [ObservableProperty]
        private string _reportData;

        [ObservableProperty]
        private bool _isGeneratingReport;

        [ObservableProperty]
        private string _selectedReportType = "attendance";

        public ReportsViewModel(IReportService reportService, IEmployeeService employeeService)
        {
            _reportService = reportService;
            _employeeService = employeeService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var departments = await _employeeService.GetDepartmentsAsync();
            Departments = new ObservableCollection<Department>(departments);

            var employees = await _employeeService.GetEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            if (IsGeneratingReport) return;

            IsGeneratingReport = true;

            try
            {
                ReportCache report = SelectedReportType switch
                {
                    "attendance" => await _reportService.GenerateAttendanceReportAsync(
                        SelectedDepartment?.DepartmentId, SelectedEmployee?.EmployeeId, SelectedPeriod),
                    "lateness" => await _reportService.GenerateLatenessReportAsync(
                        SelectedDepartment?.DepartmentId, SelectedEmployee?.EmployeeId, SelectedPeriod),
                    "summary" => await _reportService.GenerateDepartmentSummaryReportAsync(SelectedPeriod),
                    _ => null
                };

                await Task.Delay(3000);

                if (report != null)
                {
                    ReportData = report.ReportData;
                }
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь нужно показать сообщение об ошибке
                ReportData = $"Ошибка при генерации отчета: {ex.Message}";
            }
            finally
            {
                IsGeneratingReport = false;
            }
        }

        partial void OnSelectedDepartmentChanged(Department value)
        {
            _ = LoadEmployeesByDepartmentAsync();
        }

        private async Task LoadEmployeesByDepartmentAsync()
        {
            if (SelectedDepartment != null)
            {
                var employees = await _employeeService.GetEmployeesAsync(SelectedDepartment.DepartmentId);
                Employees = new ObservableCollection<Employee>(employees);
                SelectedEmployee = null;
            }
            else
            {
                var employees = await _employeeService.GetEmployeesAsync();
                Employees = new ObservableCollection<Employee>(employees);
            }
        }
    }
}