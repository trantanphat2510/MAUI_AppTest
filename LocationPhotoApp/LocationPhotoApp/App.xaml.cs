using LocationPhotoApp.Services;
using LocationPhotoApp.Views;

namespace LocationPhotoApp
{
    public partial class App : Application
    {
        public static AuthService AuthService { get; private set; }
        public App()
        {
            InitializeComponent();

            AuthService = new AuthService();

            MainPage = new ContentPage
            {
                Content = new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }
            };

            CheckAuthAndNavigate();
        }

        private async void CheckAuthAndNavigate()
        {
            await AuthService.LoadTokenAsync();

            if (!string.IsNullOrEmpty(AuthService.JwtToken))
            {
                MainPage = new AppShell();

            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
