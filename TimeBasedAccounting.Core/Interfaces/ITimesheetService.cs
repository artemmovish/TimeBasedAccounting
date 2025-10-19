using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для работы с табелями учета рабочего времени
    /// </summary>
    public interface ITimesheetService
    {
        /// <summary>
        /// Получить табели сотрудника за период
        /// </summary>
        Task<IEnumerable<Timesheet>> GetTimesheetsAsync(int employeeId, int month, int year);

        /// <summary>
        /// Добавить или обновить табель
        /// </summary>
        Task<Timesheet> AddOrUpdateTimesheetAsync(Timesheet timesheet);

        /// <summary>
        /// Добавить или обновить запись об опоздании
        /// </summary>
        Task<Lateness?> AddOrUpdateLatenessAsync(int timesheetId, int durationMinutes, string reason);
    }
}