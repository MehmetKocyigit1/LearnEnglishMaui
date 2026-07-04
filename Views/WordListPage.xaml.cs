// IngilizceProje/MAUIClient/Views/WordListPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class WordListPage : ContentPage
    {
        private readonly WordListViewModel _viewModel;

        public WordListPage(WordListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.LoadWordsCommand.CanExecute(null))
            {
                await _viewModel.LoadWordsAsync();
            }
        }
    }
}
