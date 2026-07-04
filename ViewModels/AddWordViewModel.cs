// IngilizceProje/MAUIClient/ViewModels/AddWordViewModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProje.Domain.Enums;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.ViewModels
{
    [QueryProperty(nameof(EditWordIdString), "EditWordId")]
    public partial class AddWordViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _editWordIdString = string.Empty;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private string _englishWord = string.Empty;

        [ObservableProperty]
        private string _turkishMeaning = string.Empty;

        [ObservableProperty]
        private WordTypeEnum _selectedWordType = WordTypeEnum.Noun;

        [ObservableProperty]
        private string _exampleSentence = string.Empty;

        [ObservableProperty]
        private string _exampleSentenceTranslation = string.Empty;

        [ObservableProperty]
        private bool _isFavorite;

        [ObservableProperty]
        private LearningStatusEnum _learningStatus = LearningStatusEnum.NotReviewed;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        // Populate Picker items
        public List<WordTypeEnum> WordTypes { get; } = Enum.GetValues(typeof(WordTypeEnum)).Cast<WordTypeEnum>().ToList();
        public List<LearningStatusEnum> LearningStatuses { get; } = Enum.GetValues(typeof(LearningStatusEnum)).Cast<LearningStatusEnum>().ToList();

        private int? _wordIdToEdit;

        public AddWordViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        partial void OnEditWordIdStringChanged(string value)
        {
            if (int.TryParse(value, out var id))
            {
                _wordIdToEdit = id;
                IsEditMode = true;
                MainThread.BeginInvokeOnMainThread(async () => await LoadWordToEditAsync(id));
            }
        }

        private async Task LoadWordToEditAsync(int id)
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var wordDto = await _apiService.GetWordByIdAsync(id);
                if (wordDto != null)
                {
                    EnglishWord = wordDto.EnglishWord;
                    TurkishMeaning = wordDto.TurkishMeaning;
                    SelectedWordType = wordDto.WordType;
                    ExampleSentence = wordDto.ExampleSentence;
                    ExampleSentenceTranslation = wordDto.ExampleSentenceTranslation;
                    IsFavorite = wordDto.IsFavorite;
                    LearningStatus = wordDto.LearningStatus;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Düzenleme verisi yüklenemedi: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(EnglishWord) || string.IsNullOrWhiteSpace(TurkishMeaning))
            {
                ErrorMessage = "İngilizce kelime ve Türkçe anlamı alanları zorunludur.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                bool success;
                if (IsEditMode && _wordIdToEdit.HasValue)
                {
                    var dto = new UpdateWordDto
                    {
                        Id = _wordIdToEdit.Value,
                        EnglishWord = EnglishWord.Trim(),
                        TurkishMeaning = TurkishMeaning.Trim(),
                        WordType = SelectedWordType,
                        ExampleSentence = ExampleSentence.Trim(),
                        ExampleSentenceTranslation = ExampleSentenceTranslation.Trim(),
                        IsFavorite = IsFavorite,
                        LearningStatus = LearningStatus
                    };

                    success = await _apiService.UpdateWordAsync(_wordIdToEdit.Value, dto);
                }
                else
                {
                    var dto = new CreateWordDto
                    {
                        EnglishWord = EnglishWord.Trim(),
                        TurkishMeaning = TurkishMeaning.Trim(),
                        WordType = SelectedWordType,
                        ExampleSentence = ExampleSentence.Trim(),
                        ExampleSentenceTranslation = ExampleSentenceTranslation.Trim()
                    };

                    success = await _apiService.CreateWordAsync(dto);
                }

                if (success)
                {
                    await Shell.Current.DisplayAlert("Başarılı", IsEditMode ? "Kelime güncellendi." : "Kelime eklendi.", "Tamam");
                    
                    // Clear inputs if adding another, or navigate back if editing
                    if (IsEditMode)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        EnglishWord = string.Empty;
                        TurkishMeaning = string.Empty;
                        SelectedWordType = WordTypeEnum.Noun;
                        ExampleSentence = string.Empty;
                        ExampleSentenceTranslation = string.Empty;
                        IsFavorite = false;
                        LearningStatus = LearningStatusEnum.NotReviewed;
                    }
                }
                else
                {
                    ErrorMessage = "Bir hata oluştu. Aynı İngilizce kelime daha önce eklenmiş olabilir.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kayıt hatası: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
// IngilizceProje/MAUIClient/ViewModels/AddWordViewModel.cs
