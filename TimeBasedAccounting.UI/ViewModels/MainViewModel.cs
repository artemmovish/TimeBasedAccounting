using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TimeBasedAccounting.Core.Models;

namespace TimeBasedAccounting.UI.ViewModels
{
    public class TabItemModel
    {
        public string Header { get; set; } = string.Empty;
        public object? Content { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        public User? CurrentUser { get; set; }


        public ObservableCollection<TabItemModel> Tabs { get; } = new();


        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }


        public MainViewModel(TimesheetViewModel timesheetVM, VacationViewModel vacVM, ReportsViewModel reportsVM)
        {
            Tabs.Add(new TabItemModel { Header = "Главная", Content = new TimesheetView { DataContext = timesheetVM } });
            Tabs.Add(new TabItemModel { Header = "Отпуск", Content = new VacationView { DataContext = vacVM } });
            Tabs.Add(new TabItemModel { Header = "Отчёты", Content = new ReportsView { DataContext = reportsVM } });


            LogoutCommand = ReactiveCommand.Create(() => {
                // For simplicity close app on logout
                (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.Shutdown();
            });
        }
    }
}

