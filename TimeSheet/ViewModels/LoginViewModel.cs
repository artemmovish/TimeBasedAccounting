using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimeBasedAccounting.Core.Context;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;
using TimeSheet.Windows;

namespace TimeSheet.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly AccountingDbContext _context;

        [ObservableProperty]
        private ObservableCollection<User> _users;

        [ObservableProperty]
        private User _selectedUser;

        [ObservableProperty]
        private string _login;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isUserSelectionEnabled = true;

        [ObservableProperty]
        private bool _isManualLoginEnabled = true;

        // Ссылка на PasswordBox для прямого доступа
        public PasswordBox PasswordBox { get; set; }

        public LoginViewModel(IUserService userService, AccountingDbContext context)
        {
            _userService = userService;
            _context = context;
            LoadUsers();
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                ErrorMessage = "Введите логин";
                HasError = true;
                return;
            }

            var password = PasswordBox?.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите пароль";
                HasError = true;
                return;
            }

            try
            {
                // Отключаем UI во время аутентификации
                IsUserSelectionEnabled = false;
                IsManualLoginEnabled = false;

                var result = await _userService.UserLoginAsync(Login, password);

                if (result.Status == "SUCCESS")
                {
                    // Сохраняем данные в ActiveUser
                    ActiveUser.UserId = result.UserId ?? 0;
                    ActiveUser.RoleId = result.Role == "Admin" ? false : true;
                    ActiveUser.FullName = result.FullName;

                    // Очищаем ошибки
                    ErrorMessage = string.Empty;
                    HasError = false;

                    // Закрываем окно входа с успешным результатом
                    CloseLoginWindow(true);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                    HasError = true;
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Ошибка входа: {ex.Message}";
                HasError = true;
            }
            finally
            {
                // Восстанавливаем UI
                IsUserSelectionEnabled = true;
                IsManualLoginEnabled = true;
            }
        }

        [RelayCommand]
        private void Reset()
        {
            SelectedUser = null;
            Login = string.Empty;
            PasswordBox?.Clear();
            ErrorMessage = string.Empty;
            HasError = false;
        }

        partial void OnSelectedUserChanged(User value)
        {
            if (value != null)
            {
                Login = value.Login;
                // Автоматически переводим фокус на PasswordBox при выборе пользователя
                PasswordBox?.Focus();
            }
        }

        private async void LoadUsers()
        {
            try
            {
                var users = await _context.Users
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                Users = new ObservableCollection<User>(users);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки пользователей: {ex.Message}";
                HasError = true;
            }
        }

        private void CloseLoginWindow(bool dialogResult)
        {
            // Находим и закрываем окно входа
            var loginWindow = Application.Current.Windows
                .OfType<LoginWindow>()
                .FirstOrDefault();

            if (loginWindow != null)
            {
                loginWindow.DialogResult = dialogResult;
                loginWindow.Close();
            }
        }
    }
}