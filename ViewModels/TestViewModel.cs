// IngilizceProje/MAUIClient/ViewModels/TestViewModel.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class TestViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private TestDto? _currentTest;

        [ObservableProperty]
        private bool _isAnswered;

        [ObservableProperty]
        private string _selectedOption = string.Empty;

        [ObservableProperty]
        private string _correctOption = string.Empty;

        [ObservableProperty]
        private bool _isCorrect;

        [ObservableProperty]
        private string _feedbackText = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasTest;

        [ObservableProperty]
        private string _infoMessage = string.Empty;

        [ObservableProperty]
        private string _optionA = string.Empty;
        [ObservableProperty]
        private string _optionB = string.Empty;
        [ObservableProperty]
        private string _optionC = string.Empty;
        [ObservableProperty]
        private string _optionD = string.Empty;

        // Dynamic Color properties for UI binding
        public Color OptionAColor => GetOptionColor(OptionA);
        public Color OptionBColor => GetOptionColor(OptionB);
        public Color OptionCColor => GetOptionColor(OptionC);
        public Color OptionDColor => GetOptionColor(OptionD);

        public TestViewModel(ApiService apiService)
        {
            _apiService = apiService;
            HasTest = false;
        }

        [RelayCommand]
        public async Task LoadTestAsync()
        {
            IsBusy = true;
            IsAnswered = false;
            SelectedOption = string.Empty;
            CorrectOption = string.Empty;
            FeedbackText = string.Empty;
            CurrentTest = null;
            HasTest = false;
            InfoMessage = string.Empty;

            RefreshOptionColors();

            try
            {
                var test = await _apiService.GenerateMultipleChoiceTestAsync();
                if (test != null && test.Options.Count >= 4)
                {
                    CurrentTest = test;
                    OptionA = test.Options[0];
                    OptionB = test.Options[1];
                    OptionC = test.Options[2];
                    OptionD = test.Options[3];
                    HasTest = true;
                }
                else
                {
                    InfoMessage = "Test yüklenemedi. En az 4 kelimenizin bulunması gerekmektedir.";
                }
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                RefreshOptionColors();
            }
        }

        [RelayCommand]
        private async Task SelectOptionAsync(string option)
        {
            if (IsAnswered || CurrentTest == null) return;

            SelectedOption = option;
            IsAnswered = true;
            IsBusy = true;

            RefreshOptionColors();

            try
            {
                var wordDetails = await _apiService.GetWordByIdAsync(CurrentTest.WordId);
                if (wordDetails != null)
                {
                    CorrectOption = wordDetails.TurkishMeaning;
                    IsCorrect = option.Trim().ToLower() == CorrectOption.Trim().ToLower();

                    if (IsCorrect)
                    {
                        FeedbackText = "Doğru Cevap! Tebrikler. 🎉";
                    }
                    else
                    {
                        FeedbackText = $"Yanlış Cevap. Doğrusu: '{CorrectOption}' olmalıydı.";
                    }

                    // Submit to backend
                    await _apiService.SubmitQuizResultAsync(new QuizResultDto
                    {
                        WordId = CurrentTest.WordId,
                        IsCorrect = IsCorrect
                    });
                }
                else
                {
                    FeedbackText = "Cevap doğrulanamadı.";
                }
            }
            catch (Exception ex)
            {
                FeedbackText = $"Hata: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                RefreshOptionColors();
            }
        }

        private Color GetOptionColor(string option)
        {
            if (string.IsNullOrEmpty(option)) return Color.FromArgb("#1E293B");
            if (!IsAnswered) return Color.FromArgb("#1E293B");
            
            if (option.Trim().ToLower() == CorrectOption.Trim().ToLower()) 
                return Color.FromArgb("#10B981"); // Premium Green
                
            if (option.Trim().ToLower() == SelectedOption.Trim().ToLower()) 
                return Color.FromArgb("#EF4444"); // Premium Red
                
            return Color.FromArgb("#1E293B"); // CardColor Default
        }

        private void RefreshOptionColors()
        {
            OnPropertyChanged(nameof(OptionAColor));
            OnPropertyChanged(nameof(OptionBColor));
            OnPropertyChanged(nameof(OptionCColor));
            OnPropertyChanged(nameof(OptionDColor));
        }
    }
}
