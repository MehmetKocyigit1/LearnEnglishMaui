// IngilizceProje/MAUIClient/ViewModels/WordListViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProje.Domain.Enums;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class WordListViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private List<WordDto> _allWords = new();

        [ObservableProperty]
        private ObservableCollection<WordDto> _words = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        // Filter lists for picker binding
        public List<string> StatusFilterOptions { get; } = new() { "Tümü", "Önemli (Kırmızı)", "Tekrar Gerekenler (Sarı)", "Öğrenilenler (Yeşil)", "İnceleme Yapılmayanlar (Gri)" };
        public List<string> TypeFilterOptions { get; } = new() { "Tümü", "Noun", "Verb", "Adjective", "Adverb", "Pronoun", "Conjunction", "Preposition", "Interjection", "Other" };

        [ObservableProperty]
        private string _selectedStatusFilter = "Tümü";

        [ObservableProperty]
        private string _selectedTypeFilter = "Tümü";

        public WordListViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public async Task LoadWordsAsync()
        {
            IsBusy = true;
            try
            {
                var wordList = await _apiService.GetWordsAsync();
                _allWords = wordList.ToList();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Kelimeler yüklenemedi: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            var query = _allWords.AsEnumerable();

            // Search Filter
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var search = SearchQuery.ToLower().Trim();
                query = query.Where(w => w.EnglishWord.ToLower().Contains(search) || 
                                         w.TurkishMeaning.ToLower().Contains(search));
            }

            // Status Filter
            if (SelectedStatusFilter != "Tümü")
            {
                var status = SelectedStatusFilter switch
                {
                    "Önemli (Kırmızı)" => LearningStatusEnum.VeryImportant,
                    "Tekrar Gerekenler (Sarı)" => LearningStatusEnum.NeedsReview,
                    "Öğrenilenler (Yeşil)" => LearningStatusEnum.Learned,
                    "İnceleme Yapılmayanlar (Gri)" => LearningStatusEnum.NotReviewed,
                    _ => (LearningStatusEnum?)null
                };

                if (status.HasValue)
                {
                    query = query.Where(w => w.LearningStatus == status.Value);
                }
            }

            // Type Filter
            if (SelectedTypeFilter != "Tümü" && Enum.TryParse<WordTypeEnum>(SelectedTypeFilter, out var typeEnum))
            {
                query = query.Where(w => w.WordType == typeEnum);
            }

            Words = new ObservableCollection<WordDto>(query);
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync(WordDto word)
        {
            if (word == null) return;

            var success = await _apiService.ToggleFavoriteAsync(word.Id);
            if (success)
            {
                word.IsFavorite = !word.IsFavorite;
                
                // Refresh list item view in ObservableCollection
                var index = Words.IndexOf(word);
                if (index >= 0)
                {
                    Words[index] = null!; // Trigger collection change
                    Words[index] = word;
                }
            }
        }

        [RelayCommand]
        private async Task NavigateToDetailsAsync(WordDto word)
        {
            if (word == null) return;
            // Pass WordId parameter to DetailPage
            await Shell.Current.GoToAsync($"WordDetailPage?WordId={word.Id}");
        }

        [RelayCommand]
        private async Task NavigateToAddWordAsync()
        {
            await Shell.Current.GoToAsync("AddWordPage");
        }

        [RelayCommand]
        private async Task ExportExcelAsync()
        {
            await ExportFileAsync("/api/words/export/excel", "kelimelerim.xlsx", "Excel");
        }

        [RelayCommand]
        private async Task ExportCsvAsync()
        {
            await ExportFileAsync("/api/words/export/csv", "kelimelerim.csv", "CSV");
        }

        private async Task ExportFileAsync(string url, string defaultName, string typeName)
        {
            IsBusy = true;
            try
            {
                var fileBytes = await _apiService.DownloadExportFileAsync(url);
                if (fileBytes != null)
                {
                    var filePath = Path.Combine(FileSystem.CacheDirectory, defaultName);
                    await File.WriteAllBytesAsync(filePath, fileBytes);

                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"{typeName} Raporu Paylaş",
                        File = new ShareFile(filePath)
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hata", $"{typeName} raporu indirilemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hata", $"Dışa aktarma hatası: {ex.Message}", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
