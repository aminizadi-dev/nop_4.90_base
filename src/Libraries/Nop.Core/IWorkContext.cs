using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
//COMMERCE DOMAIN REMOVED - Phase C
//Removed: using Nop.Core.Domain.Tax; using Nop.Core.Domain.Vendors;

namespace Nop.Core;

/// <summary>
/// Represents work context
/// </summary>
public partial interface IWorkContext
{
    /// <summary>
    /// Gets the current customer
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Customer> GetCurrentCustomerAsync();

    /// <summary>
    /// Sets the current customer
    /// </summary>
    /// <param name="customer">Current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SetCurrentCustomerAsync(Customer customer = null);

    /// <summary>
    /// Gets the original customer (in case the current one is impersonated)
    /// </summary>
    Customer OriginalCustomerIfImpersonated { get; }

    //COMMERCE FEATURES REMOVED - Phase C
    //Removed: GetCurrentVendorAsync, GetTaxDisplayTypeAsync, SetTaxDisplayTypeAsync (commerce features)

    /// <summary>
    /// Gets current user working language
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Language> GetWorkingLanguageAsync();

    /// <summary>
    /// Sets current user working language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SetWorkingLanguageAsync(Language language);
}