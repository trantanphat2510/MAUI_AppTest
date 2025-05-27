using LocationPhotoApi.Data;
using LocationPhotoApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LocationPhotoApi.Controllers
{
    [ApiController]
    [Route("api/qrcodedata")]
    public class QrCodeDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QrCodeDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(QRCodeData data)
        {
            if (data == null) return BadRequest();

            _context.QRCodeDatas.Add(data);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Saved successfully" });
        }
    }

}
