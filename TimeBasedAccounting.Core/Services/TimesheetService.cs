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
    public class TimesheetService : ITimesheetService
    {
        private readonly AccountingDbContext _db;
        public TimesheetService(AccountingDbContext db) => _db = db;

        public async Task<IEnumerable<Timesheet>> GetTimesheetsAsync(int employeeId, int month, int year)
        {
            return await _db.Timesheets
                .Include(t => t.AttendanceMarker)
                .Include(t => t.Lateness)
                .Where(t => t.EmployeeId == employeeId && t.Date.Month == month && t.Date.Year == year)
                .ToListAsync();
        }

        public async Task<Timesheet> AddOrUpdateTimesheetAsync(Timesheet timesheet)
        {
            var existing = await _db.Timesheets
                .FirstOrDefaultAsync(t => t.EmployeeId == timesheet.EmployeeId && t.Date == timesheet.Date);

            if (existing != null)
            {
                existing.HoursWorked = timesheet.HoursWorked;
                existing.MarkerId = timesheet.MarkerId;
                existing.RecordedBy = timesheet.RecordedBy;
                _db.Timesheets.Update(existing);
                await _db.SaveChangesAsync();
                return existing;
            }
            else
            {
                timesheet.RecordedBy = ActiveUser.UserId;
                timesheet.RecordedAt = DateTime.Now;
            }
                _db.Timesheets.Add(timesheet);
            await _db.SaveChangesAsync();
            return timesheet;
        }

        public async Task<Lateness?> AddOrUpdateLatenessAsync(int timesheetId, int durationMinutes, string reason)
        {
            var lateness = await _db.Latenesses.FirstOrDefaultAsync(l => l.TimesheetId == timesheetId);
            if (lateness != null)
            {
                lateness.DurationMinutes = durationMinutes;
                lateness.Reason = reason;
                _db.Latenesses.Update(lateness);
            }
            else
            {
                lateness = new Lateness { TimesheetId = timesheetId, DurationMinutes = durationMinutes, Reason = reason};
                _db.Latenesses.Add(lateness);
            }

            await _db.SaveChangesAsync();
            return lateness;
        }
        public async Task<IEnumerable<AttendanceMarker>> GetAttendanceMarkersAsync()
        {
            return await _db.AttendanceMarkers.ToListAsync();
        }

        public async Task<AttendanceMarker?> GetMarkerByIdAsync(int markerId)
        {
            return await _db.AttendanceMarkers.FindAsync(markerId);
        }

    }
}
