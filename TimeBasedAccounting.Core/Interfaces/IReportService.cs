using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для генерации отчетов
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Сгенерировать отчет по посещаемости
        /// </summary>
        Task<ReportCache> GenerateAttendanceReportAsync(int? departmentId, int? employeeId, DateTime period);

        /// <summary>
        /// Сгенерировать отчет по опозданиям
        /// </summary>
        Task<ReportCache> GenerateLatenessReportAsync(int? departmentId, int? employeeId, DateTime period);

        /// <summary>
        /// Сгенерировать сводный отчет по отделам
        /// </summary>
        Task<ReportCache> GenerateDepartmentSummaryReportAsync(DateTime period);
    }
}