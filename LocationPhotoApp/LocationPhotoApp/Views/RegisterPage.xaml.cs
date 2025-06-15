using LocationPhotoApp.Models;
using LocationPhotoApp.Services;

namespace LocationPhotoApp.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService = new AuthService();

    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var request = new RegisterRequest
        {
            FullName = fullNameEntry.Text,
            Email = emailEntry.Text,
            PhoneNumber = phoneEntry.Text,
            Password = passwordEntry.Text,
            ConfirmPassword = confirmPasswordEntry.Text
        };

        if (request.Password != request.ConfirmPassword)
        {
            await DisplayAlert("Lỗi", "Mật khẩu xác nhận không khớp", "OK");
            return;
        }

        var result = await _authService.RegisterAsync(request);
        if (result.Success)
        {
            await DisplayAlert("Thành công", "Đăng ký thành công", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Lỗi", result.Message ?? "Đăng ký thất bại", "OK");
        }
    }

    private void OnTermsTapped(object sender, TappedEventArgs e)
    {

    }

    private void OnPrivacyTapped(object sender, TappedEventArgs e)
    {

    }

    private void OnGoogleRegisterClicked(object sender, EventArgs e)
    {

    }

    private void OnFacebookRegisterClicked(object sender, EventArgs e)
    {

    }

    private void OnLoginTapped(object sender, TappedEventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}