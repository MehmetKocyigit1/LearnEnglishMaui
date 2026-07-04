// IngilizceProje/MAUIClient/Views/MatchingQuizPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class MatchingQuizPage : ContentPage
    {
        private readonly MatchingQuizViewModel _viewModel;

        public MatchingQuizPage(MatchingQuizViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadQuizAsync();
        }
    }
}
