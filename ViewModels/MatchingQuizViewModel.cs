// IngilizceProje/MAUIClient/ViewModels/MatchingQuizViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.ViewModels
{
    public class MatchingItem : ObservableObject
    {
        public int WordId { get; set; }
        public string EnglishWord { get; set; } = string.Empty;
        public string CorrectMeaning { get; set; } = string.Empty;

        private string _selectedMeaning = string.Empty;
        public string SelectedMeaning
        {
            get => _selectedMeaning;
            set
            {
                if (SetProperty(ref _selectedMeaning, value))
                {
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        private bool? _isCorrectMatch;
        public bool? IsCorrectMatch
        {
            get => _isCorrectMatch;
            set => SetProperty(ref _isCorrectMatch, value);
        }

        public bool IsSelected => !string.IsNullOrEmpty(SelectedMeaning);
    }

    public partial class MatchingQuizViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<MatchingItem> _quizItems = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasChecked;

        [ObservableProperty]
        private string _scoreText = string.Empty;

        [ObservableProperty]
        private bool _canLoadQuiz;

        [ObservableProperty]
        private string _infoMessage = string.Empty;

        public MatchingQuizViewModel(ApiService apiService)
        {
            _apiService = apiService;
            CanLoadQuiz = true;
        }

        [RelayCommand]
        public async Task LoadQuizAsync()
        {
            IsBusy = true;
            HasChecked = false;
            ScoreText = string.Empty;
            QuizItems.Clear();
            InfoMessage = string.Empty;

            try
            {
                var quiz = await _apiService.GenerateMatchingQuizAsync();
                if (quiz != null)
                {
                    var items = quiz.Pairs.Select(p => new MatchingItem
                    {
                        WordId = p.WordId,
                        EnglishWord = p.EnglishWord,
                        CorrectMeaning = p.TurkishMeaning
                    }).ToList();

                    QuizItems = new ObservableCollection<MatchingItem>(items);
                    CanLoadQuiz = false;
                }
                else
                {
                    InfoMessage = "Quiz yüklenemedi. Kelime sayınız yetersiz olabilir.";
                }
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CheckAnswersAsync()
        {
            if (QuizItems.Any(item => !item.IsSelected))
            {
                await Shell.Current.DisplayAlert("Uyarı", "Lütfen tüm kelimelerin Türkçe karşılıklarını yazın.", "Tamam");
                return;
            }

            IsBusy = true;
            int correctCount = 0;

            try
            {
                foreach (var item in QuizItems)
                {
                    bool isCorrect = !string.IsNullOrEmpty(item.SelectedMeaning) && 
                                     item.SelectedMeaning.Trim().Equals(item.CorrectMeaning.Trim(), StringComparison.OrdinalIgnoreCase);
                    item.IsCorrectMatch = isCorrect;

                    if (isCorrect) correctCount++;

                    // Submit each result to backend to update statistics
                    await _apiService.SubmitQuizResultAsync(new QuizResultDto
                    {
                        WordId = item.WordId,
                        IsCorrect = isCorrect
                    });
                }

                ScoreText = $"Sonuç: 5 kelimeden {correctCount} tanesini doğru yazdınız! 🎉";
                HasChecked = true;
                CanLoadQuiz = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Sonuçlar gönderilirken hata oluştu: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
