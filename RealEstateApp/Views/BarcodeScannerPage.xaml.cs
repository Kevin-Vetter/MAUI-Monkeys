using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;

namespace RealEstateApp.Views;

public partial class BarcodeScannerPage : ContentPage
{
    public BarcodeScannerPage()
    {
        InitializeComponent();
        scanner.Options = new BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false,
            TryHarder = true,
            TryInverted = true
        };
    }

    private void scanner_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results?.FirstOrDefault();
        if (result != null)
            return;
        Browser.OpenAsync(result.Value);
    }
}