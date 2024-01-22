using AppProg1.ViewModels;

namespace AppProg1.Views;

public partial class MonkeyDetails : ContentPage
{
	public MonkeyDetails(MonkeyDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}