// IngilizceProje/MAUIClient/Services/NotificationService.cs
using System;
using System.Threading.Tasks;
using Plugin.LocalNotification;

namespace IngilizceProjeMAUI.Services
{
    public class NotificationService
    {
        public async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                var isEnabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
                if (!isEnabled)
                {
                    return await LocalNotificationCenter.Current.RequestNotificationPermission();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ScheduleDailyReminderAsync(TimeSpan time)
        {
            try
            {
                await RequestPermissionsAsync();

                // Cancel existing reminder if any
                LocalNotificationCenter.Current.Cancel(1001);

                var today = DateTime.Today;
                var notifyTime = today.Add(time);

                // If scheduled time is in the past today, set for tomorrow
                if (notifyTime <= DateTime.Now)
                {
                    notifyTime = notifyTime.AddDays(1);
                }

                var request = new NotificationRequest
                {
                    NotificationId = 1001,
                    Title = "İngilizce Kelime Öğrenme",
                    Description = "Günlük kelime tekrarınızı yapmayı unutmayın! Başarı düzenli çalışmaktan geçer. 🚀",
                    BadgeNumber = 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        RepeatType = NotificationRepeat.Daily
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Notification scheduling failed: {ex.Message}");
            }
        }

        public void CancelAllReminders()
        {
            try
            {
                LocalNotificationCenter.Current.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Cancel notifications failed: {ex.Message}");
            }
        }
    }
}
