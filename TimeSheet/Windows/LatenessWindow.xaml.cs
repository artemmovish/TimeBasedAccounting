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
        public LatenessWindow(int minutes, string comment)
        {
            InitializeComponent();

            textLabel.Text = FormatLateTime(minutes, comment);
        }

        public string FormatLateTime(int minutes, string comment)
        {
            if (minutes < 0)
            {
                throw new ArgumentException("Количество минут не может быть отрицательным", nameof(minutes));
            }

            // Разделяем минуты на часы и оставшиеся минуты
            int wholeHours = (int)(minutes / 60);
            int remainingMinutes = (int)(minutes % 60);

            // Форматируем строку согласно шаблону
            return $"Длительность опоздания: {wholeHours} ч. {remainingMinutes} мин.\n\nКомментарий: {comment}";
        }
    }
}
