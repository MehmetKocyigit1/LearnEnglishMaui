// IngilizceProje/MAUIClient/ViewModels/FlashcardViewModel.cs
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class FlashcardViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private WordDto? _currentWord;

        [ObservableProperty]
        private bool _isFlipped;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasWord;

        [ObservableProperty]
        private string _emptyMessage = "Henüz kelime eklenmemiş veya yüklenecek kelime yok.";

        public FlashcardViewModel(ApiService apiService)
        {
            _apiService = apiService;
            HasWord = false;
        }

        [RelayCommand]
        public async Task LoadNextWordAsync()
        {
            IsBusy = true;
            IsFlipped = false;
            CurrentWord = null;
            HasWord = false;

            try
            {
                var word = await _apiService.GetRandomWordForReviewAsync();
                if (word != null)
                {
                    CurrentWord = word;
                    HasWord = true;
                }
                else
                {
                    EmptyMessage = "Tekrar edilecek kelime kalmadı! Harika iş çıkardınız. 🚀 Yeni kelimeler ekleyin.";
                }
            }
            catch (Exception ex)
            {
                EmptyMessage = $"Hata oluştu: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void FlipCard()
        {
            IsFlipped = true;
        }

        [RelayCommand]
        private async Task SubmitAnswerAsync(bool isCorrect)
        {
            if (CurrentWord == null) return;

            IsBusy = true;
            try
            {
                var success = await _apiService.SubmitReviewResultAsync(CurrentWord.Id, isCorrect);
                if (success)
                {
                    await LoadNextWordAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hata", "Sonuç kaydedilemedi.", "Tamam");
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
    }
}
