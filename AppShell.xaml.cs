// IngilizceProje/MAUIClient/AppShell.xaml.cs
using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using IngilizceProjeMAUI.Views;

namespace IngilizceProjeMAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages that are navigated to contextually
            Routing.RegisterRoute("WordDetailPage", typeof(WordDetailPage));
            Routing.RegisterRoute("AddWordPage", typeof(AddWordPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Check if user is logged in
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                // Redirect to login page if no auth token is stored
                await GoToAsync("//LoginPage");
            }
        }
    }
}
