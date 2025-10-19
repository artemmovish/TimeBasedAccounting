using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class  Vacation
    {
        public int VacationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; }

        // Внешние ключи
        public int EmployeeId { get; set; }
        public int VacationTypeId { get; set; }
        public int VacationStatusId { get; set; }
        public int CreatedBy { get; set; }

        // Навигационные свойства
        public Employee Employee { get; set; }
        public VacationType VacationType { get; set; }
        public VacationStatus VacationStatus { get; set; }
        public User User { get; set; }
    }
}
