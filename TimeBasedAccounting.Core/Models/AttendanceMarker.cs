using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class AttendanceMarker
    {
        public int MarkerId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        // Навигационные свойства
        public ICollection<Timesheet> Timesheets { get; set; }
    }
}
