using ClosedXML.Excel;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Services.ExportImport;

/// <summary>
/// Import manager
/// </summary>
public partial class ImportManager : IImportManager
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly ILogger _logger;
    protected readonly IMeasureService _measureService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IPictureService _pictureService;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly SecuritySettings _securitySettings;
    private static readonly string[] _separator = [">>"];

    #endregion

    #region Ctor

    public ImportManager(
        IAddressService addressService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IGenericAttributeService genericAttributeService,
        IHttpClientFactory httpClientFactory,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        ILogger logger,
        IMeasureService measureService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INopFileProvider fileProvider,
        IPictureService pictureService,
        IServiceScopeFactory serviceScopeFactory,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        SecuritySettings securitySettings)
    {
        _addressService = addressService;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dataProvider = dataProvider;
        _genericAttributeService = genericAttributeService;
        _httpClientFactory = httpClientFactory;
        _fileProvider = fileProvider;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _logger = logger;
        _measureService = measureService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _pictureService = pictureService;
        _serviceScopeFactory = serviceScopeFactory;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _securitySettings = securitySettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get excel workbook metadata
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="workbook">Excel workbook</param>
    /// <param name="languages">Languages</param>
    /// <returns>Workbook metadata</returns>
    public static WorkbookMetadata<T> GetWorkbookMetadata<T>(IXLWorkbook workbook, IList<Language> languages)
    {
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        var properties = new List<PropertyByName<T>>();
        var localizedProperties = new List<PropertyByName<T>>();
        var localizedWorksheets = new List<IXLWorksheet>();

        var poz = 1;
        while (true)
        {
            try
            {
                var cell = worksheet.Row(1).Cell(poz);

                if (string.IsNullOrEmpty(cell?.Value.ToString()))
                    break;

                poz += 1;
                properties.Add(new PropertyByName<T>(cell.Value.ToString()));
            }
            catch
            {
                break;
            }
        }

        foreach (var ws in workbook.Worksheets.Skip(1))
            if (languages.Any(l => l.UniqueSeoCode.Equals(ws.Name, StringComparison.InvariantCultureIgnoreCase)))
                localizedWorksheets.Add(ws);

        if (localizedWorksheets.Any())
        {
            // get the first worksheet in the workbook
            var localizedWorksheet = localizedWorksheets.First();

            poz = 1;
            while (true)
            {
                try
                {
                    var cell = localizedWorksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value.ToString()))
                        break;

                    poz += 1;
                    localizedProperties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }
        }

        return new WorkbookMetadata<T>
        {
            DefaultProperties = properties,
            LocalizedProperties = localizedProperties,
            DefaultWorksheet = worksheet,
            LocalizedWorksheets = localizedWorksheets
        };
    }


    /// <summary>
    /// Import customers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportCustomersFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Customer>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;

        var manager = new PropertyManager<Customer>(defaultProperties);


        var iRow = 2;
        var allRoles = await _customerService.GetAllCustomerRolesAsync();
        var countries = await _countryService.GetAllCountriesAsync();
        var states = await _stateProvinceService.GetStateProvincesAsync();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var customerGuid = manager.GetDefaultProperty("CustomerGuid").GuidValue;
            var customer = await _customerService.GetCustomerByGuidAsync(customerGuid) ??
                           await _customerService.GetCustomerByEmailAsync(manager.GetDefaultProperty("Email").StringValue);

            int? avatarPictureId = null;
            string signature = null;
            string password = null;
            string passwordSalt = null;

            var isNew = customer == null;

            if (isNew)
                customer = new Customer
                {
                    CustomerGuid = Guid.Empty.Equals(customerGuid) ? Guid.NewGuid() : customerGuid,
                    CreatedOnUtc = DateTime.UtcNow
                };

            var rolesToSave = new List<int>();

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Email":
                        customer.Email = property.StringValue;
                        break;
                    case "Username":
                        customer.Username = property.StringValue;
                        break;
                    case "IsTaxExempt":
                        customer.IsTaxExempt = property.BooleanValue;
                        break;
                    case "AffiliateId":
                        customer.AffiliateId = property.IntValue;
                        break;
                    case "Active":
                        customer.Active = property.BooleanValue;
                        break;
                    case "CustomerRoles":
                        var roles = property.StringValue.Split(", ");

                        foreach (var role in roles)
                            if (int.TryParse(role, out var roleId))
                                rolesToSave.Add(roleId);
                            else
                            {
                                var currentRole = allRoles.FirstOrDefault(r =>
                                    r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));

                                if (currentRole != null)
                                    rolesToSave.Add(currentRole.Id);
                            }
                        break;
                    case "CreatedOnUtc":
                        if (DateTime.TryParse(property.StringValue, out var date))
                            customer.CreatedOnUtc = date;
                        break;
                    case "FirstName":
                        customer.FirstName = property.StringValue;
                        break;
                    case "LastName":
                        customer.LastName = property.StringValue;
                        break;
                    case "Gender":
                        customer.Gender = property.StringValue;
                        break;
                    case "Company":
                        customer.Company = property.StringValue;
                        break;
                    case "StreetAddress":
                        customer.StreetAddress = property.StringValue;
                        break;
                    case "StreetAddress2":
                        customer.StreetAddress2 = property.StringValue;
                        break;
                    case "ZipPostalCode":
                        customer.ZipPostalCode = property.StringValue;
                        break;
                    case "City":
                        customer.City = property.StringValue;
                        break;
                    case "County":
                        customer.County = property.StringValue;
                        break;
                    case "Country":
                        if (int.TryParse(property.StringValue, out var countryId))
                            customer.CountryId = countryId;
                        else
                        {
                            var country = countries.FirstOrDefault(c =>
                                c.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (country != null)
                                customer.CountryId = country.Id;
                        }
                        break;
                    case "StateProvince":
                        if (int.TryParse(property.StringValue, out var stateId))
                            customer.StateProvinceId = stateId;
                        else
                        {
                            var state = states.FirstOrDefault(s =>
                                s.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (state != null)
                                customer.StateProvinceId = state.Id;
                        }
                        break;
                    case "Phone":
                        customer.Phone = property.StringValue;
                        break;
                    case "Fax":
                        customer.Fax = property.StringValue;
                        break;
                    case "VatNumber":
                        customer.VatNumber = property.StringValue;
                        break;
                    case "VatNumberStatus":
                        customer.VatNumberStatusId = property.IntValue;
                        break;
                    case "TimeZone":
                        customer.TimeZoneId = property.StringValue;
                        break;
                    case "AvatarPictureId":
                        avatarPictureId = property.IntValueNullable;
                        break;
                    case "Signature":
                        signature = property.StringValue;
                        break;
                    case "CustomCustomerAttributesXML":
                        customer.CustomCustomerAttributesXML = property.StringValue;
                        break;
                    case "Password":
                        password = property.StringValue;
                        break;
                    case "PasswordSalt":
                        passwordSalt = property.StringValue;
                        break;
                }
            }

            if (isNew)
                await _customerService.InsertCustomerAsync(customer);
            else
                await _customerService.UpdateCustomerAsync(customer);

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

            foreach (var roleId in rolesToSave)
            {
                var role = allRoles.FirstOrDefault(r => r.Id == roleId);

                if (role == null || customerRoles.Any(cr => cr.Id == roleId))
                    continue;

                await _customerService.AddCustomerRoleMappingAsync(
                    new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = roleId });
            }

            if (!isNew && rolesToSave.Any())
                foreach (var customerRole in customerRoles.Where(cr => !rolesToSave.Contains(cr.Id)).ToList())
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);

            if (avatarPictureId.HasValue)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.AvatarPictureIdAttribute, avatarPictureId.Value);

            if (!string.IsNullOrEmpty(signature))
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SignatureAttribute,
                    signature);

            if (_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword &&
                !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(passwordSalt))
            {
                var lastPassword = isNew ? null : await _customerService.GetCurrentPasswordAsync(customer.Id);

                if (lastPassword == null || !(lastPassword.Password.Equals(password) && lastPassword.PasswordSalt.Equals(passwordSalt)))
                    await _customerService.InsertCustomerPasswordAsync(new CustomerPassword
                    {
                        CustomerId = customer.Id,
                        Password = password,
                        PasswordSalt = passwordSalt,
                        PasswordFormat = PasswordFormat.Hashed,
                        CreatedOnUtc = DateTime.UtcNow
                    });
            }

            iRow++;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCustomers"), iRow - 2));
    }

    #endregion
}