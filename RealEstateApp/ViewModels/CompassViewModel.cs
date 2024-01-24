using System;
using System.Collections.Generic;
using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    [QueryProperty(nameof(Property), "MyProperty")]
    public class CompassPageViewModel : BaseViewModel
    {
        public CompassPageViewModel(IPropertyService service)
        {
            this.service = service;
        }

        private string _currentHeading;

        public string CurrentHeading
        {
            get { return _currentHeading; }
            set { SetProperty(ref _currentHeading, value); }
        }
        private string _currentAspect;

        public string CurrentAspect
        {
            get { return _currentAspect; }
            set { SetProperty(ref _currentAspect, value); }
        }

        private string _rotationAngle;

        public string RotationAngle
        {
            get { return _rotationAngle; }
            set { SetProperty(ref _rotationAngle, value); }
        }


        readonly IPropertyService service;
        private Property _property;
        public Property Property
        {
            get => _property;
            set { SetProperty(ref _property, value); }
        }


        private Command _getHeadingCommand;
        public ICommand GetHeadingCommand => _getHeadingCommand ??= new Command(() =>
        {
            if (!Compass.IsMonitoring)
            {
                Compass.Start(SensorSpeed.UI);
                Compass.ReadingChanged += (sender, args) =>
                {
                    Header(args);
                };
            }

            RotationAngle = "0";
            CurrentAspect = "0";
            Property.Aspect = "0";
            CurrentHeading = "0";
        });

        void Header(CompassChangedEventArgs args)
        {
            RotationAngle = (-args.Reading.HeadingMagneticNorth).ToString();
            string aspect = ConvertToText(args.Reading.HeadingMagneticNorth);
            CurrentAspect = aspect;
            Property.Aspect = aspect;
            CurrentHeading = args.Reading.HeadingMagneticNorth.ToString();
        }

        public static string ConvertToText(double heading)
        {
            // Ensure the heading is within the range [0, 360)
            heading = (heading % 360 + 360) % 360;

            if (heading >= 337.5 || heading < 22.5)
                return "North";
            else if (heading >= 22.5 && heading < 67.5)
                return "Northeast";
            else if (heading >= 67.5 && heading < 112.5)
                return "East";
            else if (heading >= 112.5 && heading < 157.5)
                return "Southeast";
            else if (heading >= 157.5 && heading < 202.5)
                return "South";
            else if (heading >= 202.5 && heading < 247.5)
                return "Southwest";
            else if (heading >= 247.5 && heading < 292.5)
                return "West";
            else if (heading >= 292.5 && heading < 337.5)
                return "Northwest";
            else
                return "Unknown"; // Handle any other cases
        }

    }
}
