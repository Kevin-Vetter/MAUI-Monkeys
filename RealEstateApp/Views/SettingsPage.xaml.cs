using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class SettingsPage : ContentPage
{
    SettingsPageViewModel _vm;
	public SettingsPage(SettingsPageViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
        this._vm = vm;
    }

    protected override async void OnAppearing()
    {
        _vm.GUID = await SecureStorage.Default.GetAsync(nameof(_vm.GUID));
    }
}