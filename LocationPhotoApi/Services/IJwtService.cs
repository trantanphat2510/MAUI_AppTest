using LocationPhotoApi.Models;

namespace LocationPhotoApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

}
