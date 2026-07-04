    // IngilizceProje/MAUIClient/ViewModels/AiChatViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Models;
using IngilizceProjeMAUI.Services;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class AiChatViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _userMessageText = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isAiTyping;

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public AiChatViewModel(ApiService apiService)
        {
            _apiService = apiService;
            
            // Add a welcome message from the Head Teacher (Başöğretmen)
            Messages.Add(new ChatMessage
            {
                IsUserMessage = false,
                Text = "Merhaba sevgili öğrencim! Ben senin Antigravity English Başöğretmeninim! 🚀 Yerçekimine meydan okuyarak İngilizce öğrenmeye hazır mısın? Listenize eklediğin kelimeler hakkında bana sorular sorabilir, pratik yapabilir ya da serbestçe sohbet edebilirsin! Söyle bakalım, bugün hangi engelleri aşıyoruz?",
                Timestamp = DateTime.Now
            });
        }

        [RelayCommand]
        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(UserMessageText) || IsBusy)
                return;

            var userMsgText = UserMessageText.Trim();
            UserMessageText = string.Empty; // Clear entry immediately for better UX

            // Add user message to history
            Messages.Add(new ChatMessage
            {
                IsUserMessage = true,
                Text = userMsgText,
                Timestamp = DateTime.Now
            });

            IsBusy = true;
            IsAiTyping = true;

            try
            {
                var request = new AiChatRequestDto { Message = userMsgText };
                var response = await _apiService.SendAiChatMessageAsync(request);

                if (response != null && !string.IsNullOrEmpty(response.Reply))
                {
                    Messages.Add(new ChatMessage
                    {
                        IsUserMessage = false,
                        Text = response.Reply,
                        Timestamp = DateTime.Now
                    });
                }
                else
                {
                    Messages.Add(new ChatMessage
                    {
                        IsUserMessage = false,
                        Text = "Üzgünüm, Başöğretmen'den cevap alınamadı. Lütfen daha sonra tekrar dene.",
                        Timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    IsUserMessage = false,
                    Text = $"Bir bağlantı hatası oluştu: {ex.Message}",
                    Timestamp = DateTime.Now
                });
            }
            finally
            {
                IsBusy = false;
                IsAiTyping = false;
            }
        }
    }
}
