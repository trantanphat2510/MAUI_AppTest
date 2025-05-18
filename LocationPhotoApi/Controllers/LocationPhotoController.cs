using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LocationPhotoController : ControllerBase
{
    private readonly string photosFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
    private readonly string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", "photos.json");

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LocationPhotoRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ImageBase64))
            return BadRequest("Invalid data.");

        try
        {
            // Decode base64 string
            byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);

            // Tạo thư mục lưu ảnh nếu chưa tồn tại
            if (!Directory.Exists(photosFolder))
                Directory.CreateDirectory(photosFolder);

            // Tạo tên file ảnh
            var fileName = $"photo_{DateTime.Now:ddMMyyyyHHmmssfff}.jpg";
            var filePath = Path.Combine(photosFolder, fileName);

            // Lưu file ảnh
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            // Tạo đối tượng lưu thông tin ảnh
            var photoInfo = new PhotoInfo
            {
                FileName = fileName,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Timestamp = DateTime.Now
            };

            // Đọc danh sách hiện có từ file JSON hoặc tạo mới nếu chưa có
            List<PhotoInfo> photosList;
            if (System.IO.File.Exists(jsonFilePath))
            {
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);
                photosList = JsonSerializer.Deserialize<List<PhotoInfo>>(jsonData) ?? new List<PhotoInfo>();
            }
            else
            {
                photosList = new List<PhotoInfo>();
            }

            // Thêm bản ghi mới
            photosList.Add(photoInfo);

            // Ghi lại file JSON
            var newJson = JsonSerializer.Serialize(photosList, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(jsonFilePath, newJson);

            // Log
            Console.WriteLine($"Received photo {fileName} at location: Lat={request.Latitude}, Lon={request.Longitude}");

            // Trả về thành công kèm tên file
            return Ok(new { Message = "Photo and location received", FileName = fileName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPhotos()
    {
        try
        {
            if (!System.IO.File.Exists(jsonFilePath))
                return Ok(new List<PhotoInfo>()); // Trả về danh sách rỗng nếu chưa có file

            var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var photosList = JsonSerializer.Deserialize<List<PhotoInfo>>(jsonData) ?? new List<PhotoInfo>();

            return Ok(photosList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

public class LocationPhotoRequest
{
    public string ImageBase64 { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class PhotoInfo
{
    public string FileName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}
