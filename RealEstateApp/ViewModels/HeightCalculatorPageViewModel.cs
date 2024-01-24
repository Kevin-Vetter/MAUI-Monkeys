using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealEstateApp.ViewModels
{
    public class HeightCalculatorPageViewModel : BaseViewModel
    {
        readonly IPropertyService service;
        public ObservableCollection<BarometerMeasurement> Measurements { get; set; } = new();
        public HeightCalculatorPageViewModel(IPropertyService service)
        {
            this.service = service;

        }
        private string _measurementLabel;

        public string MeasurementLabel
        {
            get { return _measurementLabel; }
            set { SetProperty(ref _measurementLabel, value);}
        }

        private Command _saveMeasurementDataCommand;
        public ICommand SaveMeasurementDataCommand => _saveMeasurementDataCommand ??= new Command(() => {
            Measurements.Add(new BarometerMeasurement
            {
                Pressure = CurrentPressure,
                Altitude = AltitudeInMeters,
                Label = MeasurementLabel,
                HeightChange = GetHeightChange(AltitudeInMeters)
            });
        });

        private double GetHeightChange(double alt )
        {
            if (Measurements.Count > 0)
            {
                return alt - Measurements.Last().HeightChange;
            }
            return 0;
        }

        #region section A
        private double _currentPressure;

        public double CurrentPressure
        {
            get { return _currentPressure; }
            set { SetProperty(ref _currentPressure, value); }
        }

        private double _altitudeInMeters;

        public double AltitudeInMeters
        {
            get { return _altitudeInMeters; }
            set { SetProperty(ref _altitudeInMeters, value); }
        }

        private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            double reading = e.Reading.PressureInHectopascals;
            CurrentPressure = reading;
            //https://www.dmi.dk/vind
            AltitudeInMeters = 44307.694 * (1 - Math.Pow(reading / 1020, 0.190284));
        }

        private Command _getBarometerCommand;
        public ICommand GetBarometerCommand => _getBarometerCommand ??= new Command(() =>
        {
            if (Barometer.Default.IsSupported)
            {
                if (!Barometer.Default.IsMonitoring)
                {
                    // Turn on barometer
                    Barometer.Default.Start(SensorSpeed.UI);
                    Barometer.Default.ReadingChanged += Barometer_ReadingChanged;
                }
            }
        });
        #endregion



    }
}
