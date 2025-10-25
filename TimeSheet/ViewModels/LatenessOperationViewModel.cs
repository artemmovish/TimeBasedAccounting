using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeBasedAccounting.Core.Models;
using TimeBasedAccounting.Core.Interfaces;

namespace TimeSheet.ViewModels
{
    public partial class LatenessOperationViewModel : ObservableObject
    {
        private readonly ITimesheetService _timesheetService;

        [ObservableProperty]
        private int _timesheetId;

        [ObservableProperty]
        private int _durationMinutes;

        [ObservableProperty]
        private string _reason = string.Empty;

        public LatenessOperationViewModel(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        public void SetTimesheetId(int timesheetId)
        {
            TimesheetId = timesheetId;
        }

        [RelayCommand]
        private async Task SaveLatenessAsync()
        {
            try
            {
                if (DurationMinutes <= 0)
                {
                    // Показать сообщение об ошибке
                    return;
                }

                var result = await _timesheetService.AddOrUpdateLatenessAsync(
                    TimesheetId, DurationMinutes, Reason);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                System.Windows.MessageBox.Show($"Ошибка при сохранении опоздания: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnOperationCompleted;
    }
}