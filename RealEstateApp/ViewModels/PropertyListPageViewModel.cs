using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;
public class PropertyListPageViewModel : BaseViewModel
{
    private Location _currentLocation;
    public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new();

    private readonly IPropertyService service;
    public PropertyListPageViewModel(IPropertyService service)
    {
        Title = "Property List";
        this.service = service;
    }
    private bool _isFlashlightOn;


    public bool IsFlashlightOn
    {
        get { return _isFlashlightOn; }
        set { SetProperty(ref _isFlashlightOn, value); }
    }


    bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }


    private Command _onOffFlashlightCommand;

    public ICommand OnOffFlashlightCommand => _onOffFlashlightCommand ??= new Command(async () => await OnOffFlashlightAsync());

    async Task OnOffFlashlightAsync()
    {
        if (_isFlashlightOn)
        {
            _isFlashlightOn = false;
            await Flashlight.TurnOffAsync();
        }
        else
        {
            _isFlashlightOn = true;
            await Flashlight.TurnOnAsync();
        }
    }


    private Command _sortPropertiesCommand;

    public ICommand SortPropertiesCommand => _sortPropertiesCommand ??= new Command(async () => await SortPropertiesAsync());

    async Task SortPropertiesAsync()
    {
        GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
        _currentLocation = await Geolocation.Default.GetLastKnownLocationAsync();
        if (_currentLocation == null)
        {
            _currentLocation = await Geolocation.Default.GetLocationAsync(request);
        }

        await GetPropertiesAsync();
    }

    private Command getPropertiesCommand;
    public ICommand GetPropertiesCommand => getPropertiesCommand ??= new Command(async () => await GetPropertiesAsync());

    async Task GetPropertiesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            List<Property> properties = service.GetProperties();
            List<PropertyListItem> propertyListItems = new List<PropertyListItem>();

            if (PropertiesCollection.Count != 0)
                PropertiesCollection.Clear();

            foreach (Property property in properties)
            {
                PropertyListItem item = new PropertyListItem(property);
                if (_currentLocation != null)
                {
                    item.Distance = _currentLocation.CalculateDistance(property.Latitude.Value, property.Longitude.Value, DistanceUnits.Kilometers);
                }
                propertyListItems.Add(item);
            }

            foreach (var item in propertyListItems.OrderBy(x => x.Distance))
            {
                PropertiesCollection.Add(item);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    private Command _goToDetailsCommand;
    public ICommand GoToDetailsCommand => _goToDetailsCommand ??= new Command<PropertyListItem>(async x => await GoToDetailsAsync(x));

    async Task GoToDetailsAsync(PropertyListItem propertyListItem)
    {
        if (propertyListItem == null)
            return;

        await Shell.Current.GoToAsync(nameof(PropertyDetailPage), true, new Dictionary<string, object>
        {
            {"MyPropertyListItem", propertyListItem }
        });
    }

    private Command goToAddPropertyCommand;
    public ICommand GoToAddPropertyCommand => goToAddPropertyCommand ??= new Command(async () => await GotoAddProperty());
    async Task GotoAddProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=newproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", new Property() }
        });
    }
}
