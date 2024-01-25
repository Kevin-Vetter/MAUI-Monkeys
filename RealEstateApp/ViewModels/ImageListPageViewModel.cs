using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.ViewModels
{
    [QueryProperty(nameof(Property), "MyProperty")]
    public class ImageListPageViewModel : BaseViewModel
    {
        readonly IPropertyService service;
        public ObservableCollection<string> Images { get; set; } = new();

        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                Images.Clear();
                foreach (var image in value.ImageUrls)
                {
                    Images.Add(image);
                }
                SetProperty(ref _property, value);
            }
        }
        private int _position;

        public int Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }


        public void WatchAccelerometer()
        {
            Accelerometer.Default.Start(SensorSpeed.UI);
            Accelerometer.Default.ShakeDetected += (sender, args) =>
            {
                if (Position == Images.Count-1)
                {
                    Position = 0;
                }
                else
                {
                    Position++;
                }
            };
        }

    }
}
