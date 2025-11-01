using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeSheet.Windows
{
    /// <summary>
    /// Логика взаимодействия для LatenessWindow.xaml
    /// </summary>
    public partial class LatenessWindow : Window
    {
        public LatenessWindow(decimal hours, string comment)
        {
            InitializeComponent();

            textLabel.Text = FormatLateTime(hours, comment);
        }

        public string FormatLateTime(decimal hours, string comment)
        {
            // Разделяем часы на целую и дробную части
            int wholeHours = (int)hours;
            int minutes = (int)((hours - wholeHours) * 60);

            // Форматируем строку согласно шаблону
            return $"Длительность опоздания: {wholeHours} ч. {minutes} мин.\n\nКомментарий: {comment}";
        }
    }
}
