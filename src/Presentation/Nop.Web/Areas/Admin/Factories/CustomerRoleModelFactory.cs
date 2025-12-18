using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer role model factory implementation
/// </summary>
public partial class CustomerRoleModelFactory : ICustomerRoleModelFactory
{
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CustomerRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _customerService = customerService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer role search model
    /// </summary>
    /// <param name="searchModel">Customer role search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role search model
    /// </returns>
    public virtual Task<CustomerRoleSearchModel> PrepareCustomerRoleSearchModelAsync(CustomerRoleSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged customer role list model
    /// </summary>
    /// <param name="searchModel">Customer role search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role list model
    /// </returns>
    public virtual async Task<CustomerRoleListModel> PrepareCustomerRoleListModelAsync(CustomerRoleSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customer roles
        var customerRoles = (await _customerService.GetAllCustomerRolesAsync(true)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new CustomerRoleListModel().PrepareToGridAsync(searchModel, customerRoles, () =>
        {
            return customerRoles.SelectAwait(async role =>
            {
                //fill in model values from the entity
                var customerRoleModel = role.ToModel<CustomerRoleModel>();

                //COMMERCE FEATURES REMOVED - Phase C
                //Removed: customerRoleModel.PurchasedWithProductName = (await _productService.GetProductByIdAsync(role.PurchasedWithProductId))?.Name;

                return customerRoleModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare customer role model
    /// </summary>
    /// <param name="model">Customer role model</param>
    /// <param name="customerRole">Customer role</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role model
    /// </returns>
    public virtual async Task<CustomerRoleModel> PrepareCustomerRoleModelAsync(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false)
    {
        if (customerRole != null)
        {
            //fill in model values from the entity
            model ??= customerRole.ToModel<CustomerRoleModel>();
            //COMMERCE FEATURES REMOVED - Phase C
            //Removed: model.PurchasedWithProductName = (await _productService.GetProductByIdAsync(customerRole.PurchasedWithProductId))?.Name;
        }

        //set default values for the new model
        if (customerRole == null)
            model.Active = true;

        //COMMERCE FEATURES REMOVED - Phase C
        //Removed: await _baseAdminModelFactory.PrepareTaxDisplayTypesAsync(model.TaxDisplayTypeValues, false);

        return model;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerRoleProductSearchModelAsync (commerce feature)
    //Removed: PrepareCustomerRoleProductListModelAsync (commerce feature)

    #endregion
}