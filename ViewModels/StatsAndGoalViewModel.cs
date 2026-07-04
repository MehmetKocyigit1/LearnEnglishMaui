// IngilizceProje/MAUIClient/ViewModels/StatsAndGoalViewModel.cs
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class StatsAndGoalViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private StatisticsDto? _statistics;

        [ObservableProperty]
        private string _userName = "Öğrenci";

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private int _targetCount = 10;

        [ObservableProperty]
        private TimeSpan _reminderTime = new(20, 0, 0); // Default 20:00 (8 PM)

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _reminderStatusText = string.Empty;

        public StatsAndGoalViewModel(ApiService apiService, NotificationService notificationService)
        {
            _apiService = apiService;
            _notificationService = notificationService;

            // Load saved reminder time if exists
            var savedTime = Preferences.Get("reminder_time", "20:00");
            if (TimeSpan.TryParse(savedTime, out var parsedTime))
            {
                ReminderTime = parsedTime;
                ReminderStatusText = $"Hatırlatıcı kurulu: {ReminderTime:hh\\:mm}";
            }
            else
            {
                ReminderStatusText = "Hatırlatıcı ayarlanmamış.";
            }
        }

        [RelayCommand]
        public async Task LoadStatisticsAsync()
        {
            IsBusy = true;
            try
            {
                // Request notification permission proactively
                await _notificationService.RequestPermissionsAsync();

                // Retrieve user info from secure storage
                UserName = await SecureStorage.GetAsync("user_name") ?? "Öğrenci";
                UserEmail = await SecureStorage.GetAsync("user_email") ?? "";

                Statistics = await _apiService.GetUserStatisticsAsync();
                if (Statistics != null)
                {
                    TargetCount = Statistics.DailyGoalTarget;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"İstatistikler yüklenemedi: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveDailyGoalAsync()
        {
            if (TargetCount <= 0)
            {
                await Shell.Current.DisplayAlert("Hata", "Günlük hedef 0'dan büyük olmalıdır.", "Tamam");
                return;
            }

            IsBusy = true;
            try
            {
                var success = await _apiService.SetDailyGoalAsync(TargetCount);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Başarılı", "Günlük hedefiniz başarıyla güncellendi.", "Tamam");
                    await LoadStatisticsAsync(); // Reload statistics to reflect updated target and progress
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hata", "Günlük hedef güncellenemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Hata oluştu: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveReminderAsync()
        {
            IsBusy = true;
            try
            {
                // Schedule local notification
                await _notificationService.ScheduleDailyReminderAsync(ReminderTime);
                
                // Save preference
                Preferences.Set("reminder_time", ReminderTime.ToString("hh\\:mm"));

                ReminderStatusText = $"Hatırlatıcı kuruldu: {ReminderTime:hh\\:mm}";
                await Shell.Current.DisplayAlert("Başarılı", $"Günlük hatırlatıcı saat {ReminderTime:hh\\:mm} olarak kuruldu.", "Tamam");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Hatırlatıcı kurulamadı: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void DisableReminder()
        {
            _notificationService.CancelAllReminders();
            Preferences.Remove("reminder_time");
            ReminderStatusText = "Hatırlatıcı devre dışı bırakıldı.";
            Shell.Current.DisplayAlert("Bilgi", "Tüm hatırlatıcılar iptal edildi.", "Tamam");
        }
    }
}
