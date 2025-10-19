using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.Core.Context;

namespace TimeBasedAccounting.UI.ViewModels
{
    public record TimesheetRow(string EmployeeName, DateTime Date, decimal HoursWorked, string MarkerCode);


    public class TimesheetViewModel : ReactiveObject
    {
        private readonly AccountingDbContext _db;
        public ObservableCollection<TimesheetRow> Rows { get; } = new();


        public TimesheetViewModel(AccountingDbContext db)
        {
            _db = db;
            LoadRows();
        }


        public void LoadRows()
        {
            Rows.Clear();
            var query = _db.Timesheets.Include(t => t.Employee).Include(t => t.AttendanceMarker).Take(200);
            foreach (var t in query)
            {
                Rows.Add(new TimesheetRow(
                t.Employee?.FullName ?? "-",
                t.Date,
                t.HoursWorked,
                t.AttendanceMarker?.Code ?? ""
                ));
            }
        }
    }
}
