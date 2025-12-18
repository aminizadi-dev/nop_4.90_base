using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Customers;

namespace Nop.Services.Directory;

/// <summary>
/// Represents an exchange rate plugin manager implementation
/// NOTE: Plugin infrastructure removed - this class is kept for compatibility but plugin functionality is disabled
/// </summary>
public partial class ExchangeRatePluginManager : IExchangeRatePluginManager
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;

    #endregion

    #region Ctor

    public ExchangeRatePluginManager(CurrencySettings currencySettings,
        ICustomerService customerService)
    {
        _currencySettings = currencySettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load primary active exchange rate provider
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the exchange rate provider
    /// </returns>
    public virtual async Task<IExchangeRateProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0)
    {
        // Plugin infrastructure removed - return null
        return await Task.FromResult<IExchangeRateProvider>(null);
    }

    /// <summary>
    /// Check whether the passed exchange rate provider is active
    /// </summary>
    /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IExchangeRateProvider exchangeRateProvider)
    {
        // Plugin infrastructure removed - return false
        return false;
    }

    #endregion
}