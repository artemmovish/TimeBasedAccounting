using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TimeBasedAccounting.UI.Services;
using TimeBasedAccounting.UI.Views;

namespace TimeBasedAccounting.UI.ViewModels
{

    public class LoginViewModel : ReactiveObject
    {
        private readonly IAuthService _auth;
        private readonly IServiceProvider _sp;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;


        public LoginViewModel(IAuthService auth, IServiceProvider sp)
        {
            _auth = auth;
            _sp = sp;


            LoginCommand = ReactiveCommand.CreateFromTask(DoLoginAsync);
        }


        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }


        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }


        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }


        public ReactiveCommand<Unit, Unit> LoginCommand { get; }


        private async Task DoLoginAsync()
        {
            ErrorMessage = string.Empty;
            var user = await _auth.AuthenticateAsync(Login, Password);
            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return;
            }


            // Open main window
            var mainVm = _sp.GetRequiredService<MainViewModel>();
            mainVm.CurrentUser = user;

            var mainWindow = new MainWindow
            {
                DataContext = mainVm
            };


            // Close login and show main
            var win = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            win?.Close();
            mainWindow.Show();
        }
    }

}


