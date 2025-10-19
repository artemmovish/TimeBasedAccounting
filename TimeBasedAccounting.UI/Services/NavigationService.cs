using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedAccounting.UI.Services
{
    public class NavigationService
    {
        public Window? MainWindow { get; set; }
        public void ShowMain(Window window)
        {
            MainWindow?.Close();
            MainWindow = window;
            window.Show();
        }
    }
}
