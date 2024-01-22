using AppProg1.Views;

namespace AppProg1;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(MonkeyDetails),typeof(MonkeyDetails));
	}
}
