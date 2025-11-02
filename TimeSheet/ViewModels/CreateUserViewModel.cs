// CreateUserViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TimeBasedAccounting.Core.Interfaces;
using TimeBasedAccounting.Core.Models;

namespace TimeSheet.ViewModels
{
    public partial class CreateUserViewModel : ObservableObject
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private string _login = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _title = "Создание пользователя";

        public CreateUserViewModel(IUserService userService)
        {
            _userService = userService;
        }

        [RelayCommand]
        private async Task CreateUserAsync()
        {
            if (string.IsNullOrWhiteSpace(Login) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка");
                return;
            }

            try
            {
                // В реальном приложении здесь нужно хэшировать пароль
                string passwordHash = Password; // Заменить на реальный хэш

                await _userService.AddNewUserAsync(Login, passwordHash, FullName);

                // Очистка полей после успешного создания
                Login = string.Empty;
                Password = string.Empty;
                FullName = string.Empty;

                MessageBox.Show("Пользователь успешно создан", "Успех");

                // Закрытие окна или возврат
                OnUserCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании пользователя: {ex.Message}", "Ошибка");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnOperationCancelled?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnUserCreated;
        public event EventHandler OnOperationCancelled;
    }
}