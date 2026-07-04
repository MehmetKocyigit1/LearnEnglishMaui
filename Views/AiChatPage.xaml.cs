// IngilizceProje/MAUIClient/Views/AiChatPage.xaml.cs
using System.Collections.Specialized;
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class AiChatPage : ContentPage
    {
        private readonly AiChatViewModel _viewModel;

        public AiChatPage(AiChatViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            _viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
        }

        private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && _viewModel.Messages.Count > 0)
            {
                var lastItem = _viewModel.Messages[_viewModel.Messages.Count - 1];
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        ChatCollectionView.ScrollTo(lastItem, position: ScrollToPosition.End, animate: true);
                    }
                    catch
                    {
                        // Ignore any scroll exceptions during transition
                    }
                });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        }
    }
}
