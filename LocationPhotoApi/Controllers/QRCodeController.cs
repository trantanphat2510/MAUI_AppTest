using LocationPhotoApi.Data;
using LocationPhotoApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LocationPhotoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
public class QrCodeDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public QrCodeDataController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] QRCodeData data)
    {
        if (data == null ||
            string.IsNullOrWhiteSpace(data.QrCode) ||
            data.Latitude == 0 || data.Longitude == 0)
        {
            return BadRequest("Invalid QR code data.");
        }

            data.Timestamp = DateTime.UtcNow.AddHours(7);

            _context.QRCodeDatas.Add(data);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "QR Code data saved successfully.",
            DataId = data.Id
        });
    }
}

}
