// IngilizceProje/MAUIClient/Views/AddWordPage.xaml.cs
using IngilizceProjeMAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace IngilizceProjeMAUI.Views
{
    public partial class AddWordPage : ContentPage
    {
        public AddWordPage(AddWordViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
