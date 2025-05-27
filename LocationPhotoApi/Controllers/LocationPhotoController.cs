using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using LocationPhotoApi.Data;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationPhotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string photosFolder;
        private readonly string photosFolderUrl;

        public LocationPhotoController(AppDbContext context)
        {
            _context = context;
            photosFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
            photosFolderUrl = "/photos"; // URL base ?? truy c?p ?nh

            if (!Directory.Exists(photosFolder))
                Directory.CreateDirectory(photosFolder);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LocationPhotoRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ImageBase64))
                return BadRequest("Invalid data.");

            try
            {
                byte[] imageBytes = Convert.FromBase64String(request.ImageBase64);

                string fileName = $"photo_{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                string filePath = Path.Combine(photosFolder, fileName);

                // L?u file ?nh vào th? m?c wwwroot/photos
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                var photoInfo = new PhotoInfo
                {
                    FileName = fileName,
                    ImagePath = $"{photosFolderUrl}/{fileName}", // l?u ???ng d?n ?nh t??ng ??i
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Timestamp = DateTime.Now
                };

                _context.PhotoInfos.Add(photoInfo);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Photo saved in file system and DB",
                    PhotoId = photoInfo.Id,
                    PhotoUrl = photoInfo.ImagePath // tr? URL ?nh cho client
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPhotos()
        {
            try
            {
                var photos = await _context.PhotoInfos.ToListAsync();
                return Ok(photos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
