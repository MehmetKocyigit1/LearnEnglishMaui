// IngilizceProje/MAUIClient/Services/ApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using IngilizceProje.Application.DTOs;

namespace IngilizceProjeMAUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "https://localhost:7006"
            : "http://localhost:5030";

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginAsync(LoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", dto);
                if (response.IsSuccessStatusCode)
                {
                    var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>(_jsonOptions);
                    if (tokenDto != null)
                    {
                        await SecureStorage.SetAsync("auth_token", tokenDto.Token);
                        await SecureStorage.SetAsync("user_name", tokenDto.UserName);
                        await SecureStorage.SetAsync("user_email", tokenDto.Email);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_name");
            SecureStorage.Remove("user_email");
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<WordDto>> GetWordsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/words");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<WordDto>>(_jsonOptions) 
                           ?? Array.Empty<WordDto>();
                }
                return Array.Empty<WordDto>();
            }
            catch
            {
                return Array.Empty<WordDto>();
            }
        }

        public async Task<WordDto?> GetWordByIdAsync(int id)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync($"/api/words/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WordDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateWordAsync(CreateWordDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync("/api/words", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateWordAsync(int id, UpdateWordDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"/api/words/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteWordAsync(int id)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync($"/api/words/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleFavoriteAsync(int id)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PutAsync($"/api/words/{id}/favorite", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<WordDto?> GetRandomWordForReviewAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/words/random-review");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WordDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SubmitReviewResultAsync(int id, bool isCorrect)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsync($"/api/words/{id}/review?isCorrect={isCorrect}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<MatchingQuizDto?> GenerateMatchingQuizAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/quizzes/matching");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<MatchingQuizDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TestDto?> GenerateMultipleChoiceTestAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/quizzes/multiple-choice");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SubmitQuizResultAsync(QuizResultDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync("/api/quizzes/submit-result", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<StatisticsDto?> GetUserStatisticsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/statistics/summary");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<StatisticsDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<DailyGoalDto?> GetDailyGoalAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync("/api/statistics/daily-goal");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DailyGoalDto>(_jsonOptions);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SetDailyGoalAsync(int targetCount)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsync($"/api/statistics/daily-goal?targetCount={targetCount}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public string GetExportExcelUrl()
        {
            return $"{BaseUrl}/api/words/export/excel";
        }

        public string GetExportCsvUrl()
        {
            return $"{BaseUrl}/api/words/export/csv";
        }

        public async Task<byte[]?> DownloadExportFileAsync(string relativeUrl)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync(relativeUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AiChatResponseDto?> SendAiChatMessageAsync(AiChatRequestDto dto)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync("/api/ai-chat/send", dto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AiChatResponseDto>(_jsonOptions);
                }
                return new AiChatResponseDto { Reply = $"Hata oluştu: Sunucu hatası ({response.StatusCode})" };
            }
            catch (Exception ex)
            {
                return new AiChatResponseDto { Reply = $"Bağlantı hatası: {ex.Message}" };
            }
        }
    }
}
