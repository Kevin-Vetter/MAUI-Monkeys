using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class CompassPage : ContentPage
{
	CompassViewModel _vm;
	public CompassPage(CompassViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this._vm = vm;
	}

    protected override void OnAppearing()
    {
		_vm.GetHeadingCommand.Execute(null);
    }
}