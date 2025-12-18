using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer model factory
/// </summary>
public partial interface ICustomerModelFactory
{
    /// <summary>
    /// Prepare customer search model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer search model
    /// </returns>
    Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel);

    /// <summary>
    /// Prepare paged customer list model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer list model
    /// </returns>
    Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel);

    /// <summary>
    /// Prepare customer model
    /// </summary>
    /// <param name="model">Customer model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer model
    /// </returns>
    Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer, bool excludeProperties = false);

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareRewardPointsListModelAsync (reward points feature)

    /// <summary>
    /// Prepare paged customer address list model
    /// </summary>
    /// <param name="searchModel">Customer address search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Customer customer);

    /// <summary>
    /// Prepare customer address model
    /// </summary>
    /// <param name="model">Customer address model</param>
    /// <param name="customer">Customer</param>
    /// <param name="address">Address</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address model
    /// </returns>
    Task<CustomerAddressModel> PrepareCustomerAddressModelAsync(CustomerAddressModel model,
        Customer customer, Address address, bool excludeProperties = false);

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerOrderListModelAsync (commerce feature)
    //Removed: PrepareCustomerShoppingCartListModelAsync (commerce feature)

    /// <summary>
    /// Prepare paged customer activity log list model
    /// </summary>
    /// <param name="searchModel">Customer activity log search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer activity log list model
    /// </returns>
    Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel, Customer customer);

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerBackInStockSubscriptionListModelAsync (commerce feature)

    /// <summary>
    /// Prepare online customer search model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer search model
    /// </returns>
    Task<OnlineCustomerSearchModel> PrepareOnlineCustomerSearchModelAsync(OnlineCustomerSearchModel searchModel);

    /// <summary>
    /// Prepare paged online customer list model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer list model
    /// </returns>
    Task<OnlineCustomerListModel> PrepareOnlineCustomerListModelAsync(OnlineCustomerSearchModel searchModel);

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareGdprLogSearchModelAsync (GDPR feature)
    //Removed: PrepareGdprLogListModelAsync (GDPR feature)
}