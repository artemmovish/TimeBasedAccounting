using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AccountingDbContext _db;
        public EmployeeService(AccountingDbContext db) => _db = db;

        public Task<IEnumerable<Employee>> GetEmployeesAsync(int? departmentId = null, bool onlyActive = true)
        {
            var query = _db.Employees.AsQueryable();
            if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId.Value);
            if (onlyActive) query = query.Where(e => e.IsActive);
            return query.Include(e => e.Department).ToListAsync().ContinueWith(t => t.Result.AsEnumerable());
        }

        public Task<Employee> GetEmployeeByIdAsync(int employeeId) =>
            _db.Employees.Include(e => e.Department)
                         .Include(e => e.Timesheets)
                         .Include(e => e.Vacations)
                         .FirstAsync(e => e.EmployeeId == employeeId);

        public Task<IEnumerable<Department>> GetDepartmentsAsync() =>
            _db.Departments.Include(d => d.Employees).ToListAsync().ContinueWith(t => t.Result.AsEnumerable());
    }
}
