using RealEstateApp.ViewModels;

namespace RealEstateApp.Views;

public partial class ImageListPage : ContentPage
{
	ImageListPageViewModel _vm;
	public ImageListPage(ImageListPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		_vm = vm;
	}

    protected override void OnAppearing()
    {
		_vm.WatchAccelerometer();
    }
}