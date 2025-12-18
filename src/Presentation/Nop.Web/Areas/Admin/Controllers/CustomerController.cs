using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CustomerController : BaseAdminController
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly IAddressService _addressService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerModelFactory _customerModelFactory;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IQueuedEmailService _queuedEmailService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public CustomerController(CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        EmailAccountSettings emailAccountSettings,
        IAddressService addressService,
        ICustomerActivityService customerActivityService,
        ICustomerModelFactory customerModelFactory,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEmailAccountService emailAccountService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        IImportManager importManager,
        ILocalizationService localizationService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IQueuedEmailService queuedEmailService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _emailAccountSettings = emailAccountSettings;
        _addressService = addressService;
        _customerActivityService = customerActivityService;
        _customerModelFactory = customerModelFactory;
        _customerRegistrationService = customerRegistrationService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _emailAccountService = emailAccountService;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        _importManager = importManager;
        _localizationService = localizationService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _queuedEmailService = queuedEmailService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    protected virtual async Task<string> ValidateCustomerRolesAsync(IList<CustomerRole> customerRoles, IList<CustomerRole> existingCustomerRoles)
    {
        ArgumentNullException.ThrowIfNull(customerRoles);

        ArgumentNullException.ThrowIfNull(existingCustomerRoles);

        //check ACL permission to manage customer roles
        var rolesToAdd = customerRoles.Except(existingCustomerRoles, new CustomerRoleComparerByName());
        var rolesToDelete = existingCustomerRoles.Except(customerRoles, new CustomerRoleComparerByName());
        if (rolesToAdd.Any(role => role.SystemName != NopCustomerDefaults.RegisteredRoleName) || rolesToDelete.Any())
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_ACL))
                return await _localizationService.GetResourceAsync("Admin.Customers.Customers.CustomerRolesManagingError");
        }

        //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
        //ensure that a customer is in at least one required role ('Guests' and 'Registered')
        var isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
        var isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
        if (isInGuestsRole && isInRegisteredRole)
            return await _localizationService.GetResourceAsync("Admin.Customers.Customers.GuestsAndRegisteredRolesError");
        if (!isInGuestsRole && !isInRegisteredRole)
            return await _localizationService.GetResourceAsync("Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError");

        //no errors
        return string.Empty;
    }

    protected virtual async Task<bool> SecondAdminAccountExistsAsync(Customer customer)
    {
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: [(await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName)).Id]);

        return customers.Any(c => c.Active && c.Id != customer.Id);
    }

    #endregion

    #region Customers

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> CustomerList(CustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerModelAsync(new CustomerModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(CustomerModel model, bool continueEditing, IFormCollection form)
    {
        if (!string.IsNullOrWhiteSpace(model.Email) && await _customerService.GetCustomerByEmailAsync(model.Email) != null)
            ModelState.AddModelError(string.Empty, "Email is already registered");

        if (!string.IsNullOrWhiteSpace(model.Username) && _customerSettings.UsernamesEnabled &&
            await _customerService.GetCustomerByUsernameAsync(model.Username) != null)
        {
            ModelState.AddModelError(string.Empty, "Username is already registered");
        }

        //validate customer roles
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        var newCustomerRoles = new List<CustomerRole>();
        foreach (var customerRole in allCustomerRoles)
            if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                newCustomerRoles.Add(customerRole);
        var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, new List<CustomerRole>());
        if (!string.IsNullOrEmpty(customerRolesError))
        {
            ModelState.AddModelError(string.Empty, customerRolesError);
            _notificationService.ErrorNotification(customerRolesError);
        }

        // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
        if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
            !CommonHelper.IsValidEmail(model.Email))
        {
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
        }

        if (ModelState.IsValid)
        {
            //fill entity from model
            var customer = model.ToEntity<Customer>();
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            customer.CustomerGuid = Guid.NewGuid();
            customer.CreatedOnUtc = DateTime.UtcNow;
            customer.LastActivityDateUtc = DateTime.UtcNow;
            customer.RegisteredInStoreId = currentStore.Id;

            //form fields
            if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                customer.TimeZoneId = model.TimeZoneId;
            if (_customerSettings.GenderEnabled)
                customer.Gender = model.Gender;
            if (_customerSettings.FirstNameEnabled)
                customer.FirstName = model.FirstName;
            if (_customerSettings.LastNameEnabled)
                customer.LastName = model.LastName;
            if (_customerSettings.DateOfBirthEnabled)
                customer.DateOfBirth = model.DateOfBirth;
            if (_customerSettings.CompanyEnabled)
                customer.Company = model.Company;
            if (_customerSettings.StreetAddressEnabled)
                customer.StreetAddress = model.StreetAddress;
            if (_customerSettings.StreetAddress2Enabled)
                customer.StreetAddress2 = model.StreetAddress2;
            if (_customerSettings.ZipPostalCodeEnabled)
                customer.ZipPostalCode = model.ZipPostalCode;
            if (_customerSettings.CityEnabled)
                customer.City = model.City;
            if (_customerSettings.CountyEnabled)
                customer.County = model.County;
            if (_customerSettings.CountryEnabled)
                customer.CountryId = model.CountryId;
            if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                customer.StateProvinceId = model.StateProvinceId;
            if (_customerSettings.PhoneEnabled)
                customer.Phone = model.Phone;
            if (_customerSettings.FaxEnabled)
                customer.Fax = model.Fax;

            await _customerService.InsertCustomerAsync(customer);

            //password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = await _customerRegistrationService.ChangePasswordAsync(changePassRequest);
                if (!changePassResult.Success)
                {
                    foreach (var changePassError in changePassResult.Errors)
                        _notificationService.ErrorNotification(changePassError);
                }
            }

            //customer roles
            foreach (var customerRole in newCustomerRoles)
            {
                //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                    continue;

                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
            }

            await _customerService.UpdateCustomerAsync(customer);

            //COMMERCE FEATURE REMOVED - Phase B
            //Removed: Vendor validation logic (commerce feature)
            //VendorId field is kept in Customer entity but vendor-related validation is removed

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomer"), customer.Id), customer);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        //prepare model
        model = await _customerModelFactory.PrepareCustomerModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null || customer.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerModelFactory.PrepareCustomerModelAsync(null, customer);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(CustomerModel model, bool continueEditing, IFormCollection form)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null || customer.Deleted)
            return RedirectToAction("List");

        //validate customer roles
        var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        var newCustomerRoles = new List<CustomerRole>();
        foreach (var customerRole in allCustomerRoles)
            if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                newCustomerRoles.Add(customerRole);

        var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, await _customerService.GetCustomerRolesAsync(customer));

        if (!string.IsNullOrEmpty(customerRolesError))
        {
            ModelState.AddModelError(string.Empty, customerRolesError);
            _notificationService.ErrorNotification(customerRolesError);
        }

        // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
        if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
            !CommonHelper.IsValidEmail(model.Email))
        {
            ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
        }

        if (ModelState.IsValid)
        {
            try
            {
                customer.AdminComment = model.AdminComment;
                customer.IsTaxExempt = model.IsTaxExempt;
                customer.MustChangePassword = model.MustChangePassword;

                //prevent deactivation of the last active administrator
                if (!await _customerService.IsAdminAsync(customer) || model.Active || await SecondAdminAccountExistsAsync(customer))
                    customer.Active = model.Active;
                else
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.Deactivate"));

                //email
                if (!string.IsNullOrWhiteSpace(model.Email))
                    await _customerRegistrationService.SetEmailAsync(customer, model.Email, false);
                else
                    customer.Email = model.Email;

                //username
                if (_customerSettings.UsernamesEnabled)
                {
                    if (!string.IsNullOrWhiteSpace(model.Username))
                        await _customerRegistrationService.SetUsernameAsync(customer, model.Username);
                    else
                        customer.Username = model.Username;
                }

                //VAT number (COMMERCE FEATURE REMOVED - Phase B)
                //Removed: VAT number validation using tax service
                //customer.VatNumber = model.VatNumber;
                //VAT number status validation removed as tax service is commerce-specific

                //vendor
                customer.VendorId = model.VendorId;

                //form fields
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;
                if (_customerSettings.GenderEnabled)
                    customer.Gender = model.Gender;
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.DateOfBirthEnabled)
                    customer.DateOfBirth = model.DateOfBirth;
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.StreetAddress;
                if (_customerSettings.StreetAddress2Enabled)
                    customer.StreetAddress2 = model.StreetAddress2;
                if (_customerSettings.ZipPostalCodeEnabled)
                    customer.ZipPostalCode = model.ZipPostalCode;
                if (_customerSettings.CityEnabled)
                    customer.City = model.City;
                if (_customerSettings.CountyEnabled)
                    customer.County = model.County;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                if (_customerSettings.FaxEnabled)
                    customer.Fax = model.Fax;


                var currentCustomerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer, true);

                //customer roles
                foreach (var customerRole in allCustomerRoles)
                {
                    //ensure that the current customer cannot add/remove to/from "Administrators" system role
                    //if he's not an admin himself
                    if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName &&
                        !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                        continue;

                    if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    {
                        //new role
                        if (currentCustomerRoleIds.All(roleId => roleId != customerRole.Id))
                            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                    }
                    else
                    {
                        //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await SecondAdminAccountExistsAsync(customer))
                        {
                            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteRole"));
                            continue;
                        }

                        //remove role
                        if (currentCustomerRoleIds.Any(roleId => roleId == customerRole.Id))
                            await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);
                    }
                }

                await _customerService.UpdateCustomerAsync(customer);

                //ensure that a customer with a vendor associated is not in "Administrators" role
                //otherwise, he won't have access to the other functionality in admin area
                if (await _customerService.IsAdminAsync(customer) && customer.VendorId > 0)
                {
                    customer.VendorId = 0;
                    await _customerService.UpdateCustomerAsync(customer);
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                }

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCustomer",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomer"), customer.Id), customer);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = customer.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
            }
        }

        //prepare model
        model = await _customerModelFactory.PrepareCustomerModelAsync(model, customer, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("changepassword")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ChangePassword(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        //ensure that the current customer cannot change passwords of "Administrators" if he's not an admin himself
        if (await _customerService.IsAdminAsync(customer) && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanChangePassword"));
            return RedirectToAction("Edit", new { id = customer.Id });
        }

        var changePassRequest = new ChangePasswordRequest(customer.Email,
            false, _customerSettings.DefaultPasswordFormat, model.Password);
        var changePassResult = await _customerRegistrationService.ChangePasswordAsync(changePassRequest);
        if (changePassResult.Success)
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.PasswordChanged"));
        else
            foreach (var error in changePassResult.Errors)
                _notificationService.ErrorNotification(error);

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("markVatNumberAsValid")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkVatNumberAsValid(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        await _customerService.UpdateCustomerAsync(customer);

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("markVatNumberAsInvalid")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkVatNumberAsInvalid(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        await _customerService.UpdateCustomerAsync(customer);

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("remove-affiliate")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RemoveAffiliate(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        customer.AffiliateId = 0;
        await _customerService.UpdateCustomerAsync(customer);

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RemoveBindMFA(int id)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return RedirectToAction("List");

        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

        //raise event       
        await _eventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.UnbindMFAProvider"));

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return RedirectToAction("List");

        try
        {
            //prevent attempts to delete the user, if it is the last active administrator
            if (await _customerService.IsAdminAsync(customer) && !await SecondAdminAccountExistsAsync(customer))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                return RedirectToAction("Edit", new { id = customer.Id });
            }

            //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
            if (await _customerService.IsAdminAsync(customer) && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                return RedirectToAction("Edit", new { id = customer.Id });
            }

            //get customer email before deleting customer entity to avoid problems with the changed email after deleting see CustomerSettings.SuffixDeletedCustomers settings
            var customerEmail = customer.Email;

            //delete
            await _customerService.DeleteCustomerAsync(customer);

            //remove newsletter subscriptions (if exist)
            var subscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(customerEmail);
            foreach (var subscription in subscriptions)
                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCustomer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomer"), customer.Id), customer);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Edit", new { id = customer.Id });
        }
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("impersonate")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPERSONATION)]
    public virtual async Task<IActionResult> Impersonate(int id)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return RedirectToAction("List");

        if (!customer.Active)
        {
            _notificationService.WarningNotification(
                await _localizationService.GetResourceAsync("Admin.Customers.Customers.Impersonate.Inactive"));
            return RedirectToAction("Edit", customer.Id);
        }

        //ensure that a non-admin user cannot impersonate as an administrator
        //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsAdminAsync(currentCustomer) && await _customerService.IsAdminAsync(customer))
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.NonAdminNotImpersonateAsAdminError"));
            return RedirectToAction("Edit", customer.Id);
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("Impersonation.Started",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Started.StoreOwner"), customer.Email, customer.Id), customer);
        await _customerActivityService.InsertActivityAsync(customer, "Impersonation.Started",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Started.Customer"), currentCustomer.Email, currentCustomer.Id), currentCustomer);

        //ensure login is not required
        customer.RequireReLogin = false;
        await _customerService.UpdateCustomerAsync(customer);
        await _genericAttributeService.SaveAttributeAsync<int?>(currentCustomer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, customer.Id);

        return RedirectToAction("Index", "Home", new { area = string.Empty });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("send-welcome-message")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SendWelcomeMessage(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.SendWelcomeMessage.Success"));

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [HttpPost, ActionName("Edit")]
    [FormValueRequired("resend-activation-message")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ReSendActivationMessage(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        //email validation message
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.ReSendActivationMessage.Success"));

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SendEmail(CustomerModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.Id);
        if (customer == null)
            return RedirectToAction("List");

        try
        {
            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new NopException("Customer email is empty");
            if (!CommonHelper.IsValidEmail(customer.Email))
                throw new NopException("Customer email is not valid");
            if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
                throw new NopException("Email subject is empty");
            if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
                throw new NopException("Email body is empty");

            var emailAccount = (await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
                ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault())
                ?? throw new NopException("Email account can't be loaded");
            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                EmailAccountId = emailAccount.Id,
                FromName = emailAccount.DisplayName,
                From = emailAccount.Email,
                ToName = await _customerService.GetCustomerFullNameAsync(customer),
                To = customer.Email,
                Subject = model.SendEmail.Subject,
                Body = model.SendEmail.Body,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
            };
            await _queuedEmailService.InsertQueuedEmailAsync(email);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.SendEmail.Queued"));
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
        }

        return RedirectToAction("Edit", new { id = customer.Id });
    }

    #endregion

    #region Addresses

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> AddressesSelect(CustomerAddressSearchModel searchModel)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(searchModel.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //prepare model
        var model = await _customerModelFactory.PrepareCustomerAddressListModelAsync(searchModel, customer);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressDelete(int id, int customerId)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(customerId)
            ?? throw new ArgumentException("No customer found with the specified id", nameof(customerId));

        //try to get an address with the specified id
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, id);

        if (address == null)
            return Content("No address found with the specified id");

        await _customerService.RemoveCustomerAddressAsync(customer, address);
        await _customerService.UpdateCustomerAsync(customer);

        //now delete the address record
        await _addressService.DeleteAddressAsync(address);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressCreate(int customerId)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerModelFactory.PrepareCustomerAddressModelAsync(new CustomerAddressModel(), customer, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressCreate(CustomerAddressModel model, IFormCollection form)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity<Address>();
            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await _addressService.InsertAddressAsync(address);

            await _customerService.InsertCustomerAddressAsync(customer, address);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Added"));

            return RedirectToAction("AddressEdit", new { addressId = address.Id, customerId = model.CustomerId });
        }

        //prepare model
        model = await _customerModelFactory.PrepareCustomerAddressModelAsync(model, customer, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(int addressId, int customerId)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return RedirectToAction("List");

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(addressId);
        if (address == null)
            return RedirectToAction("Edit", new { id = customer.Id });

        //prepare model
        var model = await _customerModelFactory.PrepareCustomerAddressModelAsync(null, customer, address);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddressEdit(CustomerAddressModel model, IFormCollection form)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
            return RedirectToAction("List");

        //try to get an address with the specified id
        var address = await _addressService.GetAddressByIdAsync(model.Address.Id);
        if (address == null)
            return RedirectToAction("Edit", new { id = customer.Id });

        if (ModelState.IsValid)
        {
            address = model.Address.ToEntity(address);
            await _addressService.UpdateAddressAsync(address);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

            return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
        }

        //prepare model
        model = await _customerModelFactory.PrepareCustomerAddressModelAsync(model, customer, address, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    #endregion

    #region Customer

    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public virtual async Task<IActionResult> LoadCustomerStatistics(string period)
    {
        var result = new List<object>();

        var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
        var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();
        var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };

        var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

        switch (period)
        {
            case "year":
                //year statistics
                var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                for (var i = 0; i <= 12; i++)
                {
                    result.Add(new
                    {
                        date = searchYearDateUser.Date.ToString("Y", culture),
                        value = (await _customerService.GetAllCustomersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                            customerRoleIds: searchCustomerRoleIds,
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchYearDateUser = searchYearDateUser.AddMonths(1);
                }

                break;
            case "month":
                //month statistics
                var monthAgoDt = nowDt.AddDays(-30);
                var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                for (var i = 0; i <= 30; i++)
                {
                    result.Add(new
                    {
                        date = searchMonthDateUser.Date.ToString("M", culture),
                        value = (await _customerService.GetAllCustomersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                            customerRoleIds: searchCustomerRoleIds,
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchMonthDateUser = searchMonthDateUser.AddDays(1);
                }

                break;
            case "week":
            default:
                //week statistics
                var weekAgoDt = nowDt.AddDays(-7);
                var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                for (var i = 0; i <= 7; i++)
                {
                    result.Add(new
                    {
                        date = searchWeekDateUser.Date.ToString("d dddd", culture),
                        value = (await _customerService.GetAllCustomersAsync(
                            createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                            createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                            customerRoleIds: searchCustomerRoleIds,
                            pageIndex: 0,
                            pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                    });

                    searchWeekDateUser = searchWeekDateUser.AddDays(1);
                }

                break;
        }

        return Json(result);
    }

    #endregion

    #region Activity log

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ListActivityLog(CustomerActivityLogSearchModel searchModel)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(searchModel.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //prepare model
        var model = await _customerModelFactory.PrepareCustomerActivityLogListModelAsync(searchModel, customer);

        return Json(model);
    }

    #endregion

    #region Export / Import

    [HttpPost, ActionName("ExportExcel")]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelAll(CustomerSearchModel model)
    {
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
            email: model.SearchEmail,
            username: model.SearchUsername,
            firstName: model.SearchFirstName,
            lastName: model.SearchLastName,
            dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
            monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
            company: model.SearchCompany,
            isActive: model.SearchIsActive,
            phone: model.SearchPhone,
            zipPostalCode: model.SearchZipPostalCode);

        //COMMERCE FEATURE REMOVED - Phase B
        //Removed: Export functionality (commerce feature)
        _notificationService.ErrorNotification("Export functionality is not available (commerce feature removed)");
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
    {
        var customers = new List<Customer>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            customers.AddRange(await _customerService.GetCustomersByIdsAsync(ids));
        }

        //COMMERCE FEATURE REMOVED - Phase B
        //Removed: Export functionality (commerce feature)
        _notificationService.ErrorNotification("Export functionality is not available (commerce feature removed)");
        return RedirectToAction("List");
    }

    [HttpPost, ActionName("ExportXML")]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlAll(CustomerSearchModel model)
    {
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
            email: model.SearchEmail,
            username: model.SearchUsername,
            firstName: model.SearchFirstName,
            lastName: model.SearchLastName,
            dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
            monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
            company: model.SearchCompany,
            isActive: model.SearchIsActive,
            phone: model.SearchPhone,
            zipPostalCode: model.SearchZipPostalCode);

        try
        {
            //COMMERCE FEATURE REMOVED - Phase B
            //Removed: XML export functionality (commerce feature)
            _notificationService.ErrorNotification("XML export functionality is not available (commerce feature removed)");
            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
    {
        var customers = new List<Customer>();
        if (selectedIds != null)
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            customers.AddRange(await _customerService.GetCustomersByIdsAsync(ids));
        }

        try
        {
            //COMMERCE FEATURE REMOVED - Phase B
            //Removed: XML export functionality (commerce feature)
            _notificationService.ErrorNotification("XML export functionality is not available (commerce feature removed)");
            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
    {
        //COMMERCE FEATURE REMOVED - Phase B
        //Removed: Vendor check (commerce feature)
        //if (await _workContext.GetCurrentVendorAsync() != null)
        //    return AccessDeniedView();

        try
        {
            if ((importexcelfile?.Length ?? 0) > 0)
                await _importManager.ImportCustomersFromXlsxAsync(importexcelfile.OpenReadStream());
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);

            return RedirectToAction("List");
        }
    }

    #endregion
}