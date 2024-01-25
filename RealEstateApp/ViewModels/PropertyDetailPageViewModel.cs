using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.Communication;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    private CancellationTokenSource _cts;

    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
        IsSpeaking = false;
    }

    private bool _isSpeaking;

    public bool IsSpeaking
    {
        get { return _isSpeaking; }
        set { SetProperty(ref _isSpeaking, value); }
    }


    Property property;
    public Property Property { get => property; set { SetProperty(ref property, value); } }


    Agent agent;
    public Agent Agent { get => agent; set { SetProperty(ref agent, value); } }


    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);

            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }

    private Command _textToSpeechCommand;

    public Command TextToSpeechCommand => _textToSpeechCommand ??= new Command(async () => await TextToSpeechAsync());

    async Task TextToSpeechAsync()
    {
        IsSpeaking = true;
        _cts = new CancellationTokenSource();
        await TextToSpeech.Default.SpeakAsync(Property.Description, cancelToken: _cts.Token);
        IsSpeaking = false;
    }

    private Command _cancelTextToSpeechCommand;

    public Command CancelTextToSpeechCommand => _cancelTextToSpeechCommand ??= new Command(() => CancelSpeech());


    void CancelSpeech()
    {
        IsSpeaking = false;
        if (_cts?.IsCancellationRequested ?? true)
            return;

        _cts.Cancel();
    }

    private Command editPropertyCommand;
    public ICommand EditPropertyCommand => editPropertyCommand ??= new Command(async () => await GotoEditProperty());
    async Task GotoEditProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}?mode=editproperty", true, new Dictionary<string, object>
        {
            {"MyProperty", Property }
        });
    }

    private Command _goToImageCommand;
    public ICommand GoToImageCommand => _goToImageCommand ??= new Command(async () =>
    {
        await Shell.Current.GoToAsync($"{nameof(ImageListPage)}", true, new Dictionary<string, object>
        {
            {"MyProperty", Property }
        });
    }
    );
    #region 5.1
    private Command _emailClickCommand;
    public ICommand EmailClickCommand => _emailClickCommand ??= new Command(async () =>
    {
        if (Email.Default.IsComposeSupported)
        {
            var attachmentFilePath = Path.Combine(FileSystem.AppDataDirectory, "property.txt");
            File.WriteAllText(attachmentFilePath, $"{Property.Address}");

            string subject = $"Adress";
            string body = "";
            string[] recipients = new[] { property.Vendor.Email };

            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(recipients)
            };
            message.Attachments.Add(new EmailAttachment(attachmentFilePath));
            await Email.Default.ComposeAsync(message);
        }      
    });

    private Command _phoneClickCommand;
    public ICommand PhoneClickCommand => _phoneClickCommand ??= new Command(async () =>
    {
        string choise = await Shell.Current.DisplayActionSheet(Property.Vendor.Phone, "Cancel", null, "Call", "SMS");
        switch (choise)
        {
            case "Call":
                DialVendor();
                break;
            case "SMS":
                await SendSmS();
                break;
        }
    });
    private void DialVendor()
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(Property.Vendor.Phone);

        }
        catch (FeatureNotSupportedException)
        {
        }

    }
    private async Task SendSmS()
    {
        try
        {

            var message = new SmsMessage
            {
                Recipients = new List<string> {property.Vendor.Phone},
                Body = $@"""Hello, {property.Vendor.FirstName}
About {Property.Address}
""".Remove('"')
            };
            await Sms.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
        }
    }
    #endregion

    private Command _openMapsCommand;
    public ICommand OpenMapsCommand => _openMapsCommand ??= new Command(async () =>
    {

        Location location = new(Property.Latitude.Value, Property.Longitude.Value);
        var options = new MapLaunchOptions { Name = Property.Address };

        try
        {
            await Map.Default.OpenAsync(location, options);
        }
        catch (Exception ex)
        {
            // No map application available to open
        }
    });
}
