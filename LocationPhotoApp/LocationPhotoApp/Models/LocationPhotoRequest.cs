using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationPhotoApp.Models;

public class LocationPhotoRequest
{
    public string ImageBase64 { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

