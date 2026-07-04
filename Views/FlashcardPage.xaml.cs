// IngilizceProje/MAUIClient/Views/FlashcardPage.xaml.cs
using System;
using System.Threading.Tasks;
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class FlashcardPage : ContentPage
    {
        private readonly FlashcardViewModel _viewModel;
        private bool _isAnimating = false;

        public FlashcardPage(FlashcardViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadNextWordAsync();
        }

        private async Task PerformFlipAnimationAsync()
        {
            if (_isAnimating || _viewModel == null || !_viewModel.HasWord) return;
            _isAnimating = true;

            try
            {
                // Rotate to 90 degrees (halfway, card is perpendicular to screen)
                await CardBorder.RotateYTo(90, 180, Easing.CubicIn);

                // Toggle the flip state (this swaps visible content inside the card)
                _viewModel.IsFlipped = !_viewModel.IsFlipped;

                // Move rotation instantly to -90/270 degrees to continue the rotation direction
                CardBorder.RotationY = -90;

                // Complete the rotation back to 0 degrees
                await CardBorder.RotateYTo(0, 180, Easing.CubicOut);
            }
            finally
            {
                _isAnimating = false;
            }
        }

        private async void OnCardTapped(object sender, EventArgs e)
        {
            await PerformFlipAnimationAsync();
        }

        private async void OnShowMeaningClicked(object sender, EventArgs e)
        {
            await PerformFlipAnimationAsync();
        }
    }
}
