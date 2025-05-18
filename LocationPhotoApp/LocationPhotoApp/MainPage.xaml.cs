using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LocationPhotoApp;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            bool hasLocationPermission = await CheckAndRequestPermission<Permissions.LocationWhenInUse>("vị trí");
            if (!hasLocationPermission)
                return;

            bool hasCameraPermission = await CheckAndRequestPermission<Permissions.Camera>("camera");
            if (!hasCameraPermission)
                return;

            var location = await GetCurrentLocationAsync();
            if (location == null)
            {
                await DisplayAlert("Lỗi", "Không lấy được vị trí GPS", "OK");
                return;
            }
            locationLabel.Text = $"Vị trí: Lat {location.Latitude}, Lon {location.Longitude}";

            var photo = await CapturePhotoAsync();
            if (photo == null)
            {
                await DisplayAlert("Hủy", "Bạn chưa chụp ảnh", "OK");
                return;
            }

            byte[] imageBytes = await ReadPhotoAsBytesAsync(photo);

            // Hiển thị ảnh lên UI
            photoImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            photoPlaceholder.IsVisible = false;

            statusLabel.Text = "Đang gửi dữ liệu lên server...";

            var request = new LocationPhotoRequest(Convert.ToBase64String(imageBytes), location.Latitude, location.Longitude);

            var isSuccess = await SendDataToServerAsync(request);
            statusLabel.Text = isSuccess ? "✅ Gửi dữ liệu thành công!" : "❌ Gửi dữ liệu thất bại!";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", ex.Message, "OK");
        }
    }

    private async Task<bool> CheckAndRequestPermission<T>(string permissionName) where T : Permissions.BasePermission, new()
    {
        var status = await Permissions.RequestAsync<T>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert($"Quyền {permissionName}", $"Ứng dụng cần quyền truy cập {permissionName} để tiếp tục.", "OK");
            return false;
        }
        return true;
    }

    private async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            return await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
        }
        catch
        {
            return null;
        }
    }

    private async Task<FileResult?> CapturePhotoAsync()
    {
        try
        {
            return await MediaPicker.CapturePhotoAsync();
        }
        catch
        {
            return null;
        }
    }

    private async Task<byte[]> ReadPhotoAsBytesAsync(FileResult photo)
    {
        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    private async Task<bool> SendDataToServerAsync(LocationPhotoRequest request)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync("http://10.0.2.2:5273/api/locationphoto", request);

            if (response.IsSuccessStatusCode)
                return true;

            var error = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Lỗi server", error, "OK");
            return false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi gửi dữ liệu", ex.Message, "OK");
            return false;
        }
    }
}

public class LocationPhotoRequest
{
    public string ImageBase64 { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public LocationPhotoRequest(string imageBase64, double latitude, double longitude)
    {
        ImageBase64 = imageBase64;
        Latitude = latitude;
        Longitude = longitude;
    }
}
