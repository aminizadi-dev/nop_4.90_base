using System.ComponentModel;
using Nop.Core.ComponentModel;
//COMMERCE DOMAIN REMOVED - Phase C
//Removed: using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;

namespace Nop.Core;

/// <summary>
/// Startup task for the registration custom type converters
/// </summary>
public partial class TypeConverterRegistrationStartUpTask : IStartupTask
{
    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual void Execute()
    {
        //lists
        TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
        TypeDescriptor.AddAttributes(typeof(List<decimal>), new TypeConverterAttribute(typeof(GenericListTypeConverter<decimal>)));
        TypeDescriptor.AddAttributes(typeof(List<string>), new TypeConverterAttribute(typeof(GenericListTypeConverter<string>)));

        //dictionaries
        TypeDescriptor.AddAttributes(typeof(Dictionary<int, int>), new TypeConverterAttribute(typeof(GenericDictionaryTypeConverter<int, int>)));

        //COMMERCE FEATURES REMOVED - Phase C
        //Removed: ShippingOption and PickupPoint type converters (commerce features)
    }

    /// <summary>
    /// Gets order of this startup task implementation
    /// </summary>
    public int Order => 1;
}