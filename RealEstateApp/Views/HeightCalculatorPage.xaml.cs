using RealEstateApp.ViewModels;
using System.Windows.Input;

namespace RealEstateApp.Views;

public partial class HeightCalculatorPage : ContentPage
{
	HeightCalculatorPageViewModel _vm;



    public HeightCalculatorPage(HeightCalculatorPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		this._vm = vm;
	}

	protected override void OnAppearing()
	{
		_vm.GetBarometerCommand.Execute(null);

    }
}