using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LocationPhotoApi.Data;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationPhotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string photosFolder;

        public LocationPhotoController(AppDbContext context)
        {
            _context = context;
            photosFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");

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

                var photoInfo = new PhotoInfo
                {
                    FileName = $"photo_{DateTime.Now:yyyyMMddHHmmssfff}.jpg",
                    ImageData = imageBytes,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Timestamp = DateTime.Now
                };

                _context.PhotoInfos.Add(photoInfo);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Photo saved in DB", PhotoId = photoInfo.Id });
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
