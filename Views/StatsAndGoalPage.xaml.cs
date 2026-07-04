// IngilizceProje/MAUIClient/Views/StatsAndGoalPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class StatsAndGoalPage : ContentPage
    {
        private readonly StatsAndGoalViewModel _viewModel;

        public StatsAndGoalPage(StatsAndGoalViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadStatisticsAsync();
        }
    }
}
