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
    public record VacationRow(string EmployeeName, DateTime StartDate, DateTime EndDate, string Type, string Status);


    public class VacationViewModel : ReactiveObject
    {
        private readonly AccountingDbContext _db;
        public ObservableCollection<VacationRow> Vacations { get; } = new();


        public VacationViewModel(AccountingDbContext db)
        {
            _db = db;
            Load();
        }


        public void Load()
        {
            Vacations.Clear();
            var q = _db.Vacations.Include(v => v.Employee).Include(v => v.VacationType).Include(v => v.VacationStatus).Take(200);
            foreach (var v in q)
            {
                Vacations.Add(new VacationRow(
                v.Employee?.FullName ?? "-",
                v.StartDate,
                v.EndDate,
                v.VacationType?.Name ?? "-",
                v.VacationStatus?.Name ?? "-"
                ));
            }
        }
    }
}
