using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;

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
        await TextToSpeech.Default.SpeakAsync(Property.Description,cancelToken: _cts.Token);
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
}
