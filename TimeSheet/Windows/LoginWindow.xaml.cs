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
using TimeSheet.ViewModels;

namespace TimeSheet.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // НЕ устанавливаем DataContext здесь, он будет установлен в App.xaml.cs
            // DataContext = App.ServiceProvider.GetRequiredService<LoginViewModel>();

            // Но нам все еще нужно передать PasswordBox в ViewModel
            Loaded += OnWindowLoaded;

            // Обработка нажатия Enter
            SetupEnterKeyHandling();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // После загрузки окна и установки DataContext, передаем PasswordBox в ViewModel
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.PasswordBox = PasswordBox;

                // Устанавливаем фокус
                if (viewModel.Users?.Count > 0)
                {
                    UsersComboBox?.Focus();
                }
                else
                {
                    LoginTextBox?.Focus();
                }
            }
        }

        private void SetupEnterKeyHandling()
        {
            // Обработка нажатия Enter в поле логина
            if (LoginTextBox != null)
            {
                LoginTextBox.KeyDown += (s, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Enter)
                    {
                        PasswordBox.Focus();
                    }
                };
            }

            // Обработка нажатия Enter в PasswordBox
            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (DataContext is LoginViewModel viewModel)
                    {
                        viewModel.LoginCommand.Execute(null);
                    }
                }
            };
        }

        protected override void OnClosed(System.EventArgs e)
        {
            // Если окно закрывается без установки DialogResult, устанавливаем false
            if (DialogResult != true)
            {
                DialogResult = false;
            }
            base.OnClosed(e);
        }
    }
}
