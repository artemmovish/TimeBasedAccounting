using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class VacationStatus
    {
        public int StatusId { get; set; }
        public string Name { get; set; }

        // Навигационные свойства
        public ICollection<Vacation> Vacations { get; set; }
    }
}
