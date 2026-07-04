// IngilizceProje/MAUIClient/Views/TestPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class TestPage : ContentPage
    {
        private readonly TestViewModel _viewModel;

        public TestPage(TestViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTestAsync();
        }
    }
}
