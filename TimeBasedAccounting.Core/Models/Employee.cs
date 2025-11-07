using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Внешний ключ
        public int DepartmentId { get; set; }

        // Навигационные свойства
        public Department Department { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
        public ICollection<Vacation> Vacations { get; set; }
    }
}
