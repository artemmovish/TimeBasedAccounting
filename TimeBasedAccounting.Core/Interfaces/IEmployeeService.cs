using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Interfaces
{
    /// <summary>
    /// Сервис для работы с сотрудниками
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Получить список сотрудников
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesAsync(int? departmentId = null, bool onlyActive = true);

        /// <summary>
        /// Получить сотрудника по идентификатору
        /// </summary>
        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        /// <summary>
        /// Получить список отделов
        /// </summary>
        Task<IEnumerable<Department>> GetDepartmentsAsync();
    }
}