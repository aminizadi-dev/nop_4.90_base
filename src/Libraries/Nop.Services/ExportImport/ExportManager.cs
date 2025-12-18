using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using ClosedXML.Excel;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ExportImport;

/// <summary>
/// Export manager
/// </summary>
public partial class ExportManager : IExportManager
{
    #region Fields

    protected readonly SecuritySettings _securitySettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public ExportManager(SecuritySettings securitySettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        ICountryService countryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        IStateProvinceService stateProvinceService)
    {
        _securitySettings = securitySettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _countryService = countryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _stateProvinceService = stateProvinceService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Export customer list to XLSX
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportCustomersToXlsxAsync(IList<Customer> customers)
    {
        async Task<object> getCountry(Customer customer)
        {
            var countryId = customer.CountryId;
            if (countryId == 0)
                return string.Empty;

            var country = await _countryService.GetCountryByIdAsync(countryId);
            return country?.Name ?? string.Empty;
        }

        async Task<object> getStateProvince(Customer customer)
        {
            var stateProvinceId = customer.StateProvinceId;
            if (stateProvinceId == 0)
                return string.Empty;

            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);
            return stateProvince?.Name ?? string.Empty;
        }

        var manager = new PropertyManager<Customer>(new[]
        {
            new PropertyByName<Customer>("CustomerId", (p, _) => p.Id),
            new PropertyByName<Customer>("CustomerGuid", (p, _) => p.CustomerGuid),
            new PropertyByName<Customer>("Email", (p, _) => p.Email),
            new PropertyByName<Customer>("Username", (p, _) => p.Username),
            new PropertyByName<Customer>("IsTaxExempt", (p, _) => p.IsTaxExempt),
            new PropertyByName<Customer>("AffiliateId", (p, _) => p.AffiliateId),
            new PropertyByName<Customer>("Active", (p, _) => p.Active),
            new PropertyByName<Customer>("CustomerRoles", async (p, _) => string.Join(", ",
                (await _customerService.GetCustomerRolesAsync(p)).Select(role => role.Name))),
            new PropertyByName<Customer>("IsGuest", async (p, _) => await _customerService.IsGuestAsync(p)),
            new PropertyByName<Customer>("IsRegistered", async (p, _) => await _customerService.IsRegisteredAsync(p)),
            new PropertyByName<Customer>("IsAdministrator", async (p, _) => await _customerService.IsAdminAsync(p)),
            new PropertyByName<Customer>("IsForumModerator", async (p, _) => await _customerService.IsForumModeratorAsync(p)),
            new PropertyByName<Customer>("CreatedOnUtc", (p, _) => p.CreatedOnUtc),
            new PropertyByName<Customer>("FirstName", (p, _) => p.FirstName, !_customerSettings.FirstNameEnabled),
            new PropertyByName<Customer>("LastName", (p, _) => p.LastName, !_customerSettings.LastNameEnabled),
            new PropertyByName<Customer>("Gender", (p, _) => p.Gender, !_customerSettings.GenderEnabled),
            new PropertyByName<Customer>("Company", (p, _) => p.Company, !_customerSettings.CompanyEnabled),
            new PropertyByName<Customer>("StreetAddress", (p, _) => p.StreetAddress, !_customerSettings.StreetAddressEnabled),
            new PropertyByName<Customer>("StreetAddress2", (p, _) => p.StreetAddress2, !_customerSettings.StreetAddress2Enabled),
            new PropertyByName<Customer>("ZipPostalCode", (p, _) => p.ZipPostalCode, !_customerSettings.ZipPostalCodeEnabled),
            new PropertyByName<Customer>("City", (p, _) => p.City, !_customerSettings.CityEnabled),
            new PropertyByName<Customer>("County", (p, _) => p.County, !_customerSettings.CountyEnabled),
            new PropertyByName<Customer>("Country", async (p, _) => await getCountry(p), !_customerSettings.CountryEnabled),
            new PropertyByName<Customer>("StateProvince", async (p, _) => await getStateProvince(p), !_customerSettings.StateProvinceEnabled),
            new PropertyByName<Customer>("Phone", (p, _) => p.Phone, !_customerSettings.PhoneEnabled),
            new PropertyByName<Customer>("Fax", (p, _) => p.Fax, !_customerSettings.FaxEnabled),
            new PropertyByName<Customer>("TimeZone", (p, _) => p.TimeZoneId, !_dateTimeSettings.AllowCustomersToSetTimeZone),
            new PropertyByName<Customer>("AvatarPictureId", async (p, _) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute), !_customerSettings.AllowCustomersToUploadAvatars),
            new PropertyByName<Customer>("ForumPostCount", async (p, _) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
            new PropertyByName<Customer>("Signature", async (p, _) => await _genericAttributeService.GetAttributeAsync<string>(p, NopCustomerDefaults.SignatureAttribute)),
            new PropertyByName<Customer>("CustomCustomerAttributesXML", (p, _) => p.CustomCustomerAttributesXML),
            new PropertyByName<Customer>("Password", async (p, _) =>
            {
                if (!_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword)
                    return string.Empty;

                var password = await _customerService.GetCurrentPasswordAsync(p.Id);

                if(password == null)
                    return string.Empty;

                if (password.PasswordFormat == PasswordFormat.Hashed)
                    return password.Password;

                return string.Empty;
            }, !_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword),
            new PropertyByName<Customer>("PasswordSalt", async (p, _) =>
            {
                if (!_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword)
                    return string.Empty;

                var password = await _customerService.GetCurrentPasswordAsync(p.Id);

                if(password == null)
                    return string.Empty;

                if (password.PasswordFormat == PasswordFormat.Hashed)
                    return password.PasswordSalt;

                return string.Empty;
            }, !_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword),
        }, null);

        await _customerActivityService.InsertActivityAsync("ExportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCustomers"), customers.Count));

        return await manager.ExportToXlsxAsync(customers);
    }

    /// <summary>
    /// Export customer list to XML
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportCustomersToXmlAsync(IList<Customer> customers)
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Customers");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);

        foreach (var customer in customers)
        {
            await xmlWriter.WriteStartElementAsync("Customer");
            await xmlWriter.WriteElementStringAsync("CustomerId", null, customer.Id.ToString());
            await xmlWriter.WriteElementStringAsync("CustomerGuid", null, customer.CustomerGuid.ToString());
            await xmlWriter.WriteElementStringAsync("Email", null, customer.Email);
            await xmlWriter.WriteElementStringAsync("Username", null, customer.Username);

            await xmlWriter.WriteElementStringAsync("IsTaxExempt", null, customer.IsTaxExempt.ToString());
            await xmlWriter.WriteElementStringAsync("AffiliateId", null, customer.AffiliateId.ToString());
            await xmlWriter.WriteElementStringAsync("VendorId", null, customer.VendorId.ToString());
            await xmlWriter.WriteElementStringAsync("Active", null, customer.Active.ToString());

            await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _customerService.IsGuestAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _customerService.IsRegisteredAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _customerService.IsAdminAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _customerService.IsForumModeratorAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, customer.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

            await xmlWriter.WriteElementStringAsync("FirstName", null, customer.FirstName);
            await xmlWriter.WriteElementStringAsync("LastName", null, customer.LastName);
            await xmlWriter.WriteElementStringAsync("Gender", null, customer.Gender);
            await xmlWriter.WriteElementStringAsync("Company", null, customer.Company);

            await xmlWriter.WriteElementStringAsync("CountryId", null, customer.CountryId.ToString());
            await xmlWriter.WriteElementStringAsync("StreetAddress", null, customer.StreetAddress);
            await xmlWriter.WriteElementStringAsync("StreetAddress2", null, customer.StreetAddress2);
            await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, customer.ZipPostalCode);
            await xmlWriter.WriteElementStringAsync("City", null, customer.City);
            await xmlWriter.WriteElementStringAsync("County", null, customer.County);
            await xmlWriter.WriteElementStringAsync("StateProvinceId", null, customer.StateProvinceId.ToString());
            await xmlWriter.WriteElementStringAsync("Phone", null, customer.Phone);
            await xmlWriter.WriteElementStringAsync("Fax", null, customer.Fax);
            await xmlWriter.WriteElementStringAsync("VatNumber", null, customer.VatNumber);
            await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, customer.VatNumberStatusId.ToString());
            await xmlWriter.WriteElementStringAsync("TimeZoneId", null, customer.TimeZoneId);

            await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute)).ToString());
            await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.ForumPostCountAttribute)).ToString());
            await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute));

            if (!string.IsNullOrEmpty(customer.CustomCustomerAttributesXML))
            {
                var selectedCustomerAttributes = new StringReader(customer.CustomCustomerAttributesXML);
                var selectedCustomerAttributesXmlReader = XmlReader.Create(selectedCustomerAttributes);
                await xmlWriter.WriteNodeAsync(selectedCustomerAttributesXmlReader, false);
            }

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        await _customerActivityService.InsertActivityAsync("ExportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCustomers"), customers.Count));

        return stringWriter.ToString();
    }

    #endregion
}
