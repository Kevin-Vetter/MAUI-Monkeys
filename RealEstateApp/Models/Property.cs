using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateApp.Models
{
    public class Property : BaseModel
    {
        private string _address;
        private double? _latitude;
        private double? _longitude;

        public string Id { get; set; }
        public int? Price { get; set; }
        public string Description { get; set; }
        public int? Beds { get; set; }
        public int? Baths { get; set; }
        public int? Parking { get; set; }
        public int? LandSize { get; set; }
        public string AgentId { get; set; }
        public List<string> ImageUrls { get; set; }
        public string MainImageUrl => ImageUrls?.FirstOrDefault() ?? GlobalSettings.Instance.NoImageUrl;

        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value);}
        }

        public double? Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        public double? Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }


        public Property()
        {
            Id = Guid.NewGuid().ToString();

            ImageUrls = new List<string>();
        }
    }
}
