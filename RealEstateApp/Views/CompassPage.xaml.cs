using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class CompassPage : ContentPage
{
	CompassPageViewModel _vm;
	public CompassPage(CompassPageViewModel vm)
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