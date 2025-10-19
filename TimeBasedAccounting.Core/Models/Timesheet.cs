using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class Timesheet
    {
        public int TimesheetId { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; } = 0.00m;
        public DateTime RecordedAt { get; set; }

        // Внешние ключи
        public int EmployeeId { get; set; }
        public int MarkerId { get; set; }
        public int RecordedBy { get; set; }

        // Навигационные свойства
        public Employee Employee { get; set; }
        public AttendanceMarker AttendanceMarker { get; set; }
        public User User { get; set; }
        public Lateness Lateness { get; set; } // Опциональная связь с опозданием
    }
}
