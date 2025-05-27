using LocationPhotoApi.Models;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace LocationPhotoApi.Data
{
    public class AppDbContext : DbContext
    {   
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<PhotoInfo> PhotoInfos { get; set; }
        public DbSet<QRCodeData> QRCodeDatas { get; set; }
    }
}
