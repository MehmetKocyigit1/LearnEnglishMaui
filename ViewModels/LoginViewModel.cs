// IngilizceProje/MAUIClient/ViewModels/LoginViewModel.cs
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IngilizceProje.Application.DTOs;
using IngilizceProjeMAUI.Services;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isRegisterMode;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        private void ToggleMode()
        {
            IsRegisterMode = !IsRegisterMode;
            ErrorMessage = string.Empty;
        }

        [RelayCommand]
        private async Task AuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Lütfen e-posta ve şifre alanlarını doldurun.";
                return;
            }

            if (IsRegisterMode && string.IsNullOrWhiteSpace(UserName))
            {
                ErrorMessage = "Kayıt için kullanıcı adı gereklidir.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                if (IsRegisterMode)
                {
                    var dto = new RegisterDto
                    {
                        Email = Email.Trim(),
                        UserName = UserName.Trim(),
                        Password = Password.Trim()
                    };

                    var registerSuccess = await _apiService.RegisterAsync(dto);
                    if (registerSuccess)
                    {
                        // Auto-login after registration
                        var loginSuccess = await _apiService.LoginAsync(new LoginDto
                        {
                            EmailOrUserName = Email.Trim(),
                            Password = Password.Trim()
                        });

                        if (loginSuccess)
                        {
                            await Shell.Current.GoToAsync("///MainPage");
                        }
                        else
                        {
                            IsRegisterMode = false;
                            ErrorMessage = "Kayıt başarılı. Lütfen giriş yapın.";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Kayıt başarısız. Kullanıcı adı veya e-posta kullanımda olabilir.";
                    }
                }
                else
                {
                    var dto = new LoginDto
                    {
                        EmailOrUserName = Email.Trim(),
                        Password = Password.Trim()
                    };

                    var loginSuccess = await _apiService.LoginAsync(dto);
                    if (loginSuccess)
                    {
                        await Shell.Current.GoToAsync("///MainPage");
                    }
                    else
                    {
                        ErrorMessage = "Giriş başarısız. Bilgilerinizi kontrol edin.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Bir hata oluştu: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
