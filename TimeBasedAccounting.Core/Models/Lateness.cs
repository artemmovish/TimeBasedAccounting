using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class Lateness
    {
        public int LatenessId { get; set; }
        public int DurationMinutes { get; set; }
        public string Reason { get; set; }

        // Внешний ключ
        public int TimesheetId { get; set; }

        // Навигационные свойства
        public Timesheet Timesheet { get; set; }
    }
}
