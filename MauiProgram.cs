// IngilizceProje/MAUIClient/MauiProgram.cs
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using IngilizceProjeMAUI.Services;
using IngilizceProjeMAUI.ViewModels;
using IngilizceProjeMAUI.Views;

namespace IngilizceProjeMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification() // Initialize local notification plugin
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<NotificationService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<WordListViewModel>();
            builder.Services.AddTransient<WordDetailViewModel>();
            builder.Services.AddTransient<AddWordViewModel>();
            builder.Services.AddTransient<FlashcardViewModel>();
            builder.Services.AddTransient<MatchingQuizViewModel>();
            builder.Services.AddTransient<TestViewModel>();
            builder.Services.AddTransient<StatsAndGoalViewModel>();
            builder.Services.AddTransient<AiChatViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<WordListPage>();
            builder.Services.AddTransient<WordDetailPage>();
            builder.Services.AddTransient<AddWordPage>();
            builder.Services.AddTransient<FlashcardPage>();
            builder.Services.AddTransient<MatchingQuizPage>();
            builder.Services.AddTransient<TestPage>();
            builder.Services.AddTransient<StatsAndGoalPage>();
            builder.Services.AddTransient<AiChatPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
