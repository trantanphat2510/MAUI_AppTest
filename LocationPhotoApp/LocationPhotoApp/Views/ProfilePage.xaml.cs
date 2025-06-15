using System.Text.Json;
using LocationPhotoApp.Models;
using LocationPhotoApp.Services;

namespace LocationPhotoApp.Views;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;

    public ProfilePage()
    {
        InitializeComponent();
        _authService = new AuthService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserInfoAsync();
    }

    private async Task LoadUserInfoAsync()
    {
        // Lấy token đã lưu trong SecureStorage
        var token = await SecureStorage.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            await DisplayAlert("Lỗi", "Bạn chưa đăng nhập", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Gọi API lấy thông tin người dùng
        var user = await _authService.GetProfileAsync(token);
        if (user == null)
        {
            await DisplayAlert("Lỗi", "Không thể lấy thông tin người dùng", "OK");
            return;
        }

        // Lưu thông tin user vào SecureStorage để dùng lại (tuỳ chọn)
        var userJson = JsonSerializer.Serialize(user);
        await SecureStorage.SetAsync("user_info", userJson);

        // Hiển thị thông tin lên UI
        fullNameLabel.Text = user.FullName;
        emailLabel.Text = user.Email;
        phoneLabel.Text = user.PhoneNumber;
        createdAtLabel.Text = user.CreatedAt.ToLocalTime().ToString("g");
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private void EditProfileButton_Clicked(object sender, EventArgs e)
    {

    }
}
