using AppProg1.Models;
using AppProg1.ViewModels;
using System.Collections.ObjectModel;

namespace AppProg1.Views;

public partial class MainPage : ContentPage
{
    private MainPageViewModel _viewModel;
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

    }

    protected override void OnAppearing()
    {
        _viewModel.GetMonkeysCommand.Execute(null);
    }
}

