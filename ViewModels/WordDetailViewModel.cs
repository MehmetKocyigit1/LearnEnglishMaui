// IngilizceProje/MAUIClient/ViewModels/WordDetailViewModel.cs
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.ViewModels
{
    [QueryProperty(nameof(WordIdString), "WordId")]
    public partial class WordDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _wordIdString = string.Empty;

        [ObservableProperty]
        private WordDto? _word;

        [ObservableProperty]
        private bool _isBusy;

        public WordDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Triggered when WordIdString query parameter is set
        partial void OnWordIdStringChanged(string value)
        {
            if (int.TryParse(value, out var id))
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadWordDetailsAsync(id));
            }
        }

        public async Task LoadWordDetailsAsync(int id)
        {
            IsBusy = true;
            try
            {
                Word = await _apiService.GetWordByIdAsync(id);
                if (Word == null)
                {
                    await Shell.Current.DisplayAlert("Hata", "Kelime bulunamadı.", "Tamam");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Detaylar yüklenemedi: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync()
        {
            if (Word == null) return;

            var success = await _apiService.ToggleFavoriteAsync(Word.Id);
            if (success)
            {
                Word.IsFavorite = !Word.IsFavorite;
                OnPropertyChanged(nameof(Word)); // Force UI refresh
            }
        }

        [RelayCommand]
        private async Task EditWordAsync()
        {
            if (Word == null) return;
            // Navigate to AddWordPage in Edit Mode
            await Shell.Current.GoToAsync($"AddWordPage?EditWordId={Word.Id}");
        }

        [RelayCommand]
        private async Task DeleteWordAsync()
        {
            if (Word == null) return;

            var confirm = await Shell.Current.DisplayAlert("Silme Onayı", $"'{Word.EnglishWord}' kelimesini silmek istediğinize emin misiniz?", "Evet", "Hayır");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var success = await _apiService.DeleteWordAsync(Word.Id);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Başarılı", "Kelime başarıyla silindi.", "Tamam");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hata", "Kelime silinemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Silme işlemi sırasında hata oluştu: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
