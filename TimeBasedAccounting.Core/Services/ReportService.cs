using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;



namespace TimeBasedAccounting.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly AccountingDbContext _db;
        public ReportService(AccountingDbContext db) => _db = db;

        // Уже реализован AttendanceReport
        public async Task<ReportCache> GenerateAttendanceReportAsync(int? departmentId, int? employeeId, DateTime period)
        {
            var query = _db.Timesheets.Include(t => t.Employee)
                                      .Include(t => t.AttendanceMarker)
                                      .AsQueryable();

            if (departmentId.HasValue) query = query.Where(t => t.Employee.DepartmentId == departmentId);
            if (employeeId.HasValue) query = query.Where(t => t.EmployeeId == employeeId);
            query = query.Where(t => t.Date.Year == period.Year && t.Date.Month == period.Month);

            var data = await query.Select(t => new
            {
                t.Date,
                Employee = t.Employee.FullName,
                Marker = t.AttendanceMarker.Code,
                t.HoursWorked
            }).ToListAsync();

            var reportJson = System.Text.Json.JsonSerializer.Serialize(data);

            var report = new ReportCache
            {
                ReportType = "Attendance",
                ReportData = reportJson,
                Period = period,
                GeneratedAt = DateTime.Now,
                GeneratedBy = ActiveUser.UserId
            };

            _db.ReportCaches.Add(report);
            await _db.SaveChangesAsync();

            return report;
        }

        // ------------------------------
        // Отчет по опозданиям
        // ------------------------------
        public async Task<ReportCache> GenerateLatenessReportAsync(int? departmentId, int? employeeId, DateTime period)
        {
            var query = _db.Latenesses
                .Include(l => l.Timesheet)
                    .ThenInclude(t => t.Employee)
                        .ThenInclude(e => e.Department)
                .AsQueryable();

            if (departmentId.HasValue) query = query.Where(l => l.Timesheet.Employee.DepartmentId == departmentId);
            if (employeeId.HasValue) query = query.Where(l => l.Timesheet.EmployeeId == employeeId);

            query = query.Where(l => l.Timesheet.Date.Year == period.Year && l.Timesheet.Date.Month == period.Month);

            var data = await query.Select(l => new
            {
                Date = l.Timesheet.Date,
                Employee = l.Timesheet.Employee.FullName,
                Department = l.Timesheet.Employee.Department.Name,
                DurationMinutes = l.DurationMinutes,
                Reason = l.Reason
            }).ToListAsync();

            var reportJson = System.Text.Json.JsonSerializer.Serialize(data);

            var report = new ReportCache
            {
                ReportType = "Lateness",
                ReportData = reportJson,
                Period = period,
                GeneratedAt = DateTime.Now,
                GeneratedBy = ActiveUser.UserId
            };

            _db.ReportCaches.Add(report);
            await _db.SaveChangesAsync();

            return report;
        }

        // ------------------------------
        // Сводный отчет по отделам
        // ------------------------------
        public async Task<ReportCache> GenerateDepartmentSummaryReportAsync(DateTime period)
        {
            var query = _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Timesheets)
                .Include(e => e.Vacations)
                .AsQueryable();

            // Сводка: по отделам
            var data = await query
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Department = g.Key.Name,
                    EmployeesCount = g.Count(),
                    TotalHoursWorked = g.SelectMany(e => e.Timesheets)
                                        .Where(t => t.Date.Year == period.Year && t.Date.Month == period.Month)
                                        .Sum(t => t.HoursWorked),
                    TotalLatenessMinutes = g.SelectMany(e => e.Timesheets)
                                            .Where(t => t.Date.Year == period.Year && t.Date.Month == period.Month)
                                            .Where(t => t.Lateness != null)
                                            .Sum(t => t.Lateness.DurationMinutes),
                    TotalVacationDays = g.SelectMany(e => e.Vacations)
                     .Where(v => v.StartDate.Month == period.Month && v.StartDate.Year == period.Year)
                     .Sum(v => (v.EndDate - v.StartDate).Days + 1)

                }).ToListAsync();

            var reportJson = System.Text.Json.JsonSerializer.Serialize(data);

            var report = new ReportCache
            {
                ReportType = "DepartmentSummary",
                ReportData = reportJson,
                Period = period,
                GeneratedAt = DateTime.Now,
                GeneratedBy = ActiveUser.UserId
            };

            _db.ReportCaches.Add(report);
            await _db.SaveChangesAsync();

            return report;
        }
    }

}
