// IngilizceProje/MAUIClient/Views/WordDetailPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class WordDetailPage : ContentPage
    {
        public WordDetailPage(WordDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
