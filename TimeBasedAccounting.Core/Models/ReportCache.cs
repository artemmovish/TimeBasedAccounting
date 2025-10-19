using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core.Models
{
    public class ReportCache
    {
        public int ReportId { get; set; }
        public string ReportType { get; set; }
        public string ReportData { get; set; } // JSON строка
        public DateTime Period { get; set; }
        public DateTime GeneratedAt { get; set; }

        // Внешний ключ
        public int GeneratedBy { get; set; }

        // Навигационные свойства
        public User User { get; set; }
    }
}
