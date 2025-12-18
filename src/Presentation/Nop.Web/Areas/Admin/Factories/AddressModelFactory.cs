using Nop.Core.Domain.Common;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the address model factory implementation
/// </summary>
public partial class AddressModelFactory : IAddressModelFactory
{
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;

    #endregion

    #region Ctor

    public AddressModelFactory(
        IBaseAdminModelFactory baseAdminModelFactory)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare address model
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare available countries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

        //prepare available states
        await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);

        if (address == null)
            return;
    }

    #endregion
}