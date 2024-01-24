using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class AddEditPropertyPage : ContentPage
{
    AddEditPropertyPageViewModel _vm;
    public AddEditPropertyPage(AddEditPropertyPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        this._vm = vm;
	}

    protected override void OnAppearing()
    {
        _vm.CheckConnectivityCommand.Execute(null);
    }
}