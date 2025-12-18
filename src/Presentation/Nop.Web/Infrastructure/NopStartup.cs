using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Framework.Factories;
using Nop.Web.Infrastructure.Installation;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Represents the registering services on application startup
/// </summary>
public partial class NopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //installation localization service
        services.AddScoped<IInstallationLocalizationService, InstallationLocalizationService>();

        //common factories (infrastructure only)
        services.AddScoped<ILocalizedModelFactory, LocalizedModelFactory>();
        services.AddScoped<IStoreMappingSupportedModelFactory, StoreMappingSupportedModelFactory>();

        //admin factories (infrastructure only)
        services.AddScoped<IAclSupportedModelFactory, AclSupportedModelFactory>();
        services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
        services.AddScoped<IActivityLogModelFactory, ActivityLogModelFactory>();
        services.AddScoped<IAddressModelFactory, AddressModelFactory>();
        services.AddScoped<ICommonModelFactory, CommonModelFactory>();
        services.AddScoped<ICountryModelFactory, CountryModelFactory>();
        services.AddScoped<ICustomerModelFactory, CustomerModelFactory>();
        services.AddScoped<ICustomerRoleModelFactory, CustomerRoleModelFactory>();
        services.AddScoped<IEmailAccountModelFactory, EmailAccountModelFactory>();
        services.AddScoped<IHomeModelFactory, HomeModelFactory>();
        services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
        services.AddScoped<ILogModelFactory, LogModelFactory>();
        services.AddScoped<IMeasureModelFactory, MeasureModelFactory>();
        services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
        services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
        services.AddScoped<ISecurityModelFactory, SecurityModelFactory>();
        services.AddScoped<ISettingModelFactory, SettingModelFactory>();
        services.AddScoped<IStoreModelFactory, StoreModelFactory>();
        services.AddScoped<ITranslationModelFactory, TranslationModelFactory>();

        //COMMERCE FACTORIES REMOVED - Phase B
        //Removed: DiscountSupportedModelFactory, AffiliateModelFactory, BlogModelFactory, CampaignModelFactory, CategoryModelFactory, CheckoutAttributeModelFactory, DiscountModelFactory, FilterLevelValueModelFactory, ForumModelFactory, GiftCardModelFactory, ManufacturerModelFactory, NewsModelFactory, OrderModelFactory, PaymentModelFactory, PollModelFactory, ProductModelFactory, ProductAttributeModelFactory, ProductReviewModelFactory, ReportModelFactory, RecurringPaymentModelFactory, ReturnRequestModelFactory, ReviewTypeModelFactory, ShippingModelFactory, ShoppingCartModelFactory, SpecificationAttributeModelFactory, TaxModelFactory, TopicModelFactory, VendorAttributeModelFactory, VendorModelFactory, WidgetModelFactory, MenuModelFactory

        //factories (infrastructure only)
        services.AddScoped<Factories.IAddressModelFactory, Factories.AddressModelFactory>();
        services.AddScoped<Factories.ICommonModelFactory, Factories.CommonModelFactory>();
        services.AddScoped<Factories.ICountryModelFactory, Factories.CountryModelFactory>();
        services.AddScoped<Factories.ICustomerModelFactory, Factories.CustomerModelFactory>();
        services.AddScoped<Factories.IMenuModelFactory, Factories.MenuModelFactory>();
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: services.AddScoped<Factories.IExternalAuthenticationModelFactory, Factories.ExternalAuthenticationModelFactory>();
        services.AddScoped<Factories.IJsonLdModelFactory, Factories.JsonLdModelFactory>();
        services.AddScoped<Factories.ISitemapModelFactory, Factories.SitemapModelFactory>();

        //COMMERCE FACTORIES REMOVED - Phase B
        //Removed: BlogModelFactory, CatalogModelFactory, CheckoutModelFactory, ForumModelFactory, FilterLevelValueModelFactory, NewsModelFactory, OrderModelFactory, PollModelFactory, PrivateMessagesModelFactory, ProductModelFactory, ReturnRequestModelFactory, ShoppingCartModelFactory, TopicModelFactory, VendorModelFactory, MenuModelFactory

        //helpers classes
        services.AddScoped<ISummernoteHelper, SummernoteHelper>();
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public virtual void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 2002;
}