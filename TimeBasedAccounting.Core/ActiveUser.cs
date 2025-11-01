using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.Core
{
    public static class ActiveUser
    {
        public static int UserId { get; set; }
        // Admin = 0, Accountant = 1
        public static bool RoleId { get; set; }
        public static string FullName { get; set; }

    }
}
