using LocationPhotoApi.Models;

namespace LocationPhotoApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequest request);
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
    }
}
