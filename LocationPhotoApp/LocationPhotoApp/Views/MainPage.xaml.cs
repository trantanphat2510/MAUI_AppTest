using LocationPhotoApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace LocationPhotoApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
    }

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
            //10.0.2.2:5273 localhost:5273
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

    private async void OnScanQRCodeClicked(object sender, EventArgs e)
    {
        try
        {
            // Kiểm tra quyền vị trí
            bool hasLocationPermission = await CheckAndRequestPermission<Permissions.LocationWhenInUse>("vị trí");
            if (!hasLocationPermission)
                return;

            // Lấy vị trí hiện tại
            var location = await GetCurrentLocationAsync();
            if (location == null)
            {
                await DisplayAlert("Lỗi", "Không lấy được vị trí GPS", "OK");
                return;
            }
            locationLabel.Text = $"Vị trí: Lat {location.Latitude}, Lon {location.Longitude}";

            var scanPage = new ZXing.Net.Maui.Controls.CameraBarcodeReaderView
            {
                IsDetecting = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var overlay = new ContentPage { Content = scanPage };

            EventHandler<ZXing.Net.Maui.BarcodeDetectionEventArgs> handler = null;

            handler = async (s, args) =>
            {
                if (args?.Results?.Any() == true)
                {
                    scanPage.IsDetecting = false;
                    scanPage.BarcodesDetected -= handler;

                    var result = args.Results.FirstOrDefault();
                    if (result == null) return;

                    var text = result.Value;

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        qrResultLabel.Text = $"📌 QR Data: {text}";
                        await Navigation.PopModalAsync();

                        statusLabel.Text = "Đang gửi dữ liệu QR đến server...";
                    });

                    // Gửi dữ liệu QR và vị trí lên server
                    var isSuccess = await SendQRCodeDataToServerAsync(text, location.Latitude, location.Longitude);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        statusLabel.Text = isSuccess ? "✅ Gửi dữ liệu QR thành công!" : "❌ Gửi dữ liệu QR thất bại!";
                    });
                }
            };

            scanPage.BarcodesDetected += handler;

            await Navigation.PushModalAsync(overlay);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", ex.Message, "OK");
        }
    }

    private async Task<bool> SendQRCodeDataToServerAsync(string qrText, double latitude, double longitude)
    {
        try
        {
            var httpClient = new HttpClient();
            var url = "http://10.0.2.2:5273/api/qrcodedata";

            var data = new
            {
                QrCode = qrText,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

}


