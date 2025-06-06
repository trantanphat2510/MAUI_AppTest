using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LocationPhotoApp.Models;

namespace LocationPhotoApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public string JwtToken { get; private set; }

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5273/") };
        }

        // Đăng ký người dùng mới
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
                if (response.IsSuccessStatusCode)
                    return (true, "Đăng ký thành công");

                var error = await response.Content.ReadAsStringAsync();
                return (false, $"Đăng ký thất bại: {error}");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi gọi API đăng ký: {ex.Message}");
            }
        }

        // Đăng nhập và lưu token
        public async Task<(bool Success, string Message)> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, $"Đăng nhập thất bại: {error}");
                }

                var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
                JwtToken = content?.Token;

                if (string.IsNullOrEmpty(JwtToken))
                    return (false, "Token không hợp lệ");

                await SecureStorage.SetAsync("jwt_token", JwtToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

                return (true, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi gọi API đăng nhập: {ex.Message}");
            }
        }

        // Đăng xuất và xóa token
        public async Task LogoutAsync()
        {
            JwtToken = null;
            await SecureStorage.SetAsync("jwt_token", string.Empty);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // Tải lại token nếu có (khi mở lại app)
        public async Task LoadTokenAsync()
        {
            var savedToken = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(savedToken))
            {
                JwtToken = savedToken;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
            }
        }

        // Lấy profile người dùng với token JWT
        public async Task<User> GetProfileAsync(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync("api/auth/profile");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user;
        }

    }

    // Model phản hồi khi login thành công
    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
