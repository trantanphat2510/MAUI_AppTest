using LocationPhotoApp.Models;
using LocationPhotoApp.Services;

namespace LocationPhotoApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService = new AuthService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var request = new LoginRequest
        {
            Email = emailEntry.Text,
            Password = passwordEntry.Text
        };

        var result = await _authService.LoginAsync(request);
        if (result.Success)
        {
            await DisplayAlert("Thành công", "Đăng nhập thành công", "OK");

            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Lỗi", result.Message ?? "Email hoặc mật khẩu không đúng", "OK");
        }
    }

    private void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {

    }

    private void OnGoogleLoginClicked(object sender, EventArgs e)
    {

    }

    private void OnFacebookLoginClicked(object sender, EventArgs e)
    {

    }

    private void OnSignUpTapped(object sender, TappedEventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new RegisterPage());
    }
}