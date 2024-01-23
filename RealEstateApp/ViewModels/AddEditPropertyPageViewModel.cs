﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel
{

    private bool _isCheckingLocation;
    private bool _btnHomeIsEnabled;

    public bool BtnHomeIsEnabled
    {
        get => _btnHomeIsEnabled;
        set { SetProperty(ref _btnHomeIsEnabled, value); }
    }

    public bool IsCheckingLocation
    {
        get { return IsCheckingLocation; }
        set { SetProperty(ref _isCheckingLocation, value); }
    }


    readonly IPropertyService service;

    public AddEditPropertyPageViewModel(IPropertyService service)
    {
        this.service = service;
        Agents = new ObservableCollection<Agent>(service.GetAgents());
    }

    public string Mode { get; set; }

    #region PROPERTIES
    public ObservableCollection<Agent> Agents { get; }

    private Property _property;
    public Property Property
    {
        get => _property;
        set
        {
            SetProperty(ref _property, value);
            Title = Mode == "newproperty" ? "Add Property" : "Edit Property";

            if (_property.AgentId != null)
            {
                SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
            }
        }
    }

    private Agent _selectedAgent;
    public Agent SelectedAgent
    {
        get => _selectedAgent;
        set
        {
            if (Property != null)
            {
                _selectedAgent = value;
                Property.AgentId = _selectedAgent?.Id;
            }
        }
    }

    string statusMessage;
    public string StatusMessage
    {
        get { return statusMessage; }
        set { SetProperty(ref statusMessage, value); }
    }

    Color statusColor;
    public Color StatusColor
    {
        get { return statusColor; }
        set { SetProperty(ref statusColor, value); }
    }
    #endregion


    private Command _checkConnectivityCommand;

    public ICommand CheckConnectivityCommand => _checkConnectivityCommand ??= new Command(async () => await CheckConnectivityAsync());

    private async Task CheckConnectivityAsync()
    {
        //do not use NetworkAccess.Local?
        //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/networking?view=net-maui-8.0&tabs=android
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            BtnHomeIsEnabled = true;
        }
        else
        {
            BtnHomeIsEnabled = false;
            await Shell.Current.DisplayAlert("Connection Problem", "No internet connection detected, restore connection to gain full accessibility ", "OK");
        }

        Connectivity.ConnectivityChanged += async (sender, args) => await CheckConnectivityAsync();

    }

    private Command _setPropertyLocationCommand;

    public ICommand SetPropertyLocationCommand => _setPropertyLocationCommand ??= new Command(async () => await SetPropertyLocationAsync());

    private async Task SetPropertyLocationAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Property.Address))
            {
                await Shell.Current.DisplayAlert("Whopm Whopm", "Please enter an address", "ok");
            }
            else
            {
                Location location = (await Geocoding.GetLocationsAsync(Property.Address)).FirstOrDefault();
                Property.Latitude = location.Latitude;
                Property.Longitude = location.Longitude;
            }
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Whopm Whopm", "An error wasn't handle properly", "ok :(");
        }
    }

    private Command _getCurrentLocationCommand;

    public ICommand GetCurrentLocationCommand => _getCurrentLocationCommand ??= new Command(async () => await GetCurrentLocationAsync());
    private async Task GetCurrentLocationAsync()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));



            Location location = await Geolocation.Default.GetLocationAsync(request);
            Property.Latitude = location.Latitude;
            Property.Longitude = location.Longitude;

            var placemarks = await Geocoding.GetPlacemarksAsync(location);
            Placemark placemark = placemarks.FirstOrDefault();
            Property.Address = $"{placemark.Thoroughfare} {placemark.FeatureName}, {placemark.Locality} {placemark.PostalCode} {placemark.CountryName}";
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Exeption Thrown", ex.Message, "ok");
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    private Command savePropertyCommand;
    public ICommand SavePropertyCommand => savePropertyCommand ??= new Command(async () => await SaveProperty());

    private async Task SaveProperty()
    {
        if (IsValid() == false)
        {
            Vibration.Vibrate(TimeSpan.FromSeconds(5));
            StatusMessage = "Please fill in all required fields";
            StatusColor = Colors.Red;
        }
        else
        {
            service.SaveProperty(Property);
            await Shell.Current.GoToAsync("///propertylist");
        }
    }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Property.Address)
            || Property.Beds == null
            || Property.Price == null
            || Property.AgentId == null)
            return false;
        return true;
    }

    private Command cancelSaveCommand;
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(async () =>
    {
        Vibration.Cancel();
        await Shell.Current.GoToAsync("..");
    });

}
