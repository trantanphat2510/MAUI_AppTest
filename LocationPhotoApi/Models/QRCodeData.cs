namespace LocationPhotoApi.Models
{
    public class QRCodeData
    {
        public int Id { get; set; }
        public string QrCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
