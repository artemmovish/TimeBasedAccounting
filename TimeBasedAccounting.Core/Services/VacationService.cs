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
    public class VacationService : IVacationService
    {
        private readonly AccountingDbContext _db;
        public VacationService(AccountingDbContext db) => _db = db;

        public async Task<IEnumerable<Vacation>> GetVacationsAsync(int? employeeId = null, int? statusId = null, DateTime? period = null)
        {
            var query = _db.Vacations.Include(v => v.Employee)
                                     .Include(v => v.VacationType)
                                     .Include(v => v.VacationStatus)
                                     .AsQueryable();

            if (employeeId.HasValue) query = query.Where(v => v.EmployeeId == employeeId.Value);
            if (statusId.HasValue) query = query.Where(v => v.VacationStatusId == statusId.Value);
            if (period.HasValue) query = query.Where(v => v.StartDate <= period && v.EndDate >= period);

            return await query.ToListAsync();
        }

        public async Task<Vacation> CreateVacationAsync(Vacation vacation)
        {
            _db.Vacations.Add(vacation);
            await _db.SaveChangesAsync();
            return vacation;
        }

        public async Task<Vacation> UpdateVacationStatusAsync(int vacationId, int newStatusId)
        {
            var vacation = await _db.Vacations.FirstAsync(v => v.VacationId == vacationId);
            vacation.VacationStatusId = newStatusId;
            _db.Vacations.Update(vacation);
            await _db.SaveChangesAsync();
            return vacation;
        }

        public async Task<IEnumerable<VacationType>> GetVacationTypesAsync()
        {
            return await _db.VacationTypes.ToListAsync();
        }

        public async Task<IEnumerable<VacationStatus>> GetVacationStatusesAsync()
        {
            return await _db.VacationStatuses.ToListAsync();
        }
    }
}
