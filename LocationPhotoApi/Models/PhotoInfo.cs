namespace YourNamespace.Models
{
    public class PhotoInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string? ImagePath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
