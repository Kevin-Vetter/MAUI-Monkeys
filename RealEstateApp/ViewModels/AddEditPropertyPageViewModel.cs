using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(Property), "MyProperty")]
public class AddEditPropertyPageViewModel : BaseViewModel
{

    private bool _isCheckingLocation;


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


    private Command _setPropertyLocation;

    public ICommand SetPropertyLocation => _setPropertyLocation ??= new Command(async () => await SetPropertyLocationAsync());

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
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Whopm Whopm", "An error wasn't handle properly", "ok :(");
        }
    }

    private Command _getCurrentLocation;

    public ICommand GetCurrentLocation => _getCurrentLocation ??= new Command(async () => await GetCurrentLocationAsync());
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
    public ICommand CancelSaveCommand => cancelSaveCommand ??= new Command(async () => await Shell.Current.GoToAsync(".."));
}
