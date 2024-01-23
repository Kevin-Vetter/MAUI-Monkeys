using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateApp.Models;
public class PropertyListItem : BaseModel
{


    private Property _property;
    private double _distance;

    public Property Property
    {
        get => _property;
        set { SetProperty(ref _property, value); }
    }

    public double Distance
    {
        get => _distance;
        set { SetProperty(ref _distance, value);}
    }

    public PropertyListItem(Property property)
    {
        Property = property;
    }
}
