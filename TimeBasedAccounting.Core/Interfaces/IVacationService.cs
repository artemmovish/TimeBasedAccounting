using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для работы с отпусками
    /// </summary>
    public interface IVacationService
    {
        /// <summary>
        /// Получить список отпусков
        /// </summary>
        Task<IEnumerable<Vacation>> GetVacationsAsync(int? employeeId = null, int? statusId = null, DateTime? period = null);

        /// <summary>
        /// Создать запись об отпуске
        /// </summary>
        Task<Vacation> CreateVacationAsync(Vacation vacation);

        /// <summary>
        /// Обновить статус отпуска
        /// </summary>
        Task<Vacation> UpdateVacationStatusAsync(int vacationId, int newStatusId);
    }
}