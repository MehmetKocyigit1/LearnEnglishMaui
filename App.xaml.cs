namespace IngilizceProjeMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light; // Force Light Mode (White background) by default

            MainPage = new AppShell();
        }
    }
}
