using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Caching;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using TaskScheduler = Nop.Services.ScheduleTasks.TaskScheduler;

namespace Nop.Web.Framework.Infrastructure;

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
        //file provider
        services.AddScoped<INopFileProvider, NopFileProvider>();

        //web helper
        services.AddScoped<IWebHelper, WebHelper>();

        //user agent helper
        services.AddScoped<IUserAgentHelper, UserAgentHelper>();

        //plugins
        services.AddScoped<IPluginService, PluginService>();

        //static cache manager
        var appSettings = Singleton<AppSettings>.Instance;
        var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();

        services.AddTransient(typeof(IConcurrentCollection<>), typeof(ConcurrentTrie<>));

        services.AddSingleton<ICacheKeyManager, CacheKeyManager>();
        services.AddScoped<IShortTermCacheManager, PerRequestCacheManager>();

        if (distributedCacheConfig.Enabled)
        {
            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddScoped<IStaticCacheManager, MemoryDistributedCacheManager>();
                    services.AddScoped<ICacheKeyService, MemoryDistributedCacheManager>();
                    break;
                case DistributedCacheType.SqlServer:
                    services.AddScoped<IStaticCacheManager, MsSqlServerCacheManager>();
                    services.AddScoped<ICacheKeyService, MsSqlServerCacheManager>();
                    break;
                case DistributedCacheType.Redis:
                    services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>();
                    services.AddScoped<IStaticCacheManager, RedisCacheManager>();
                    services.AddScoped<ICacheKeyService, RedisCacheManager>();
                    break;
                case DistributedCacheType.RedisSynchronizedMemory:
                    services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>();
                    services.AddSingleton<ISynchronizedMemoryCache, RedisSynchronizedMemoryCache>();
                    services.AddSingleton<IStaticCacheManager, SynchronizedMemoryCacheManager>();
                    services.AddScoped<ICacheKeyService, SynchronizedMemoryCacheManager>();
                    break;
            }

            services.AddSingleton<ILocker, DistributedCacheLocker>();
        }
        else
        {
            services.AddSingleton<ILocker, MemoryCacheLocker>();
            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
            services.AddScoped<ICacheKeyService, MemoryCacheManager>();
        }

        //work context
        services.AddScoped<IWorkContext, WebWorkContext>();

        //store context
        services.AddScoped<IStoreContext, WebStoreContext>();

        //services (infrastructure only)
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ISearchTermService, SearchTermService>();
        services.AddScoped<IGenericAttributeService, GenericAttributeService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerRegistrationService, CustomerRegistrationService>();
        //COMMERCE SERVICE REMOVED - Phase C
        //Removed: services.AddScoped<ICustomerReportService, CustomerReportService>(); (commerce feature)
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAclService, AclService>();
        services.AddScoped<IGeoLookupService, GeoLookupService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IMeasureService, MeasureService>();
        services.AddScoped<IStateProvinceService, StateProvinceService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreMappingService, StoreMappingService>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<ILocalizedEntityService, LocalizedEntityService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IMessageTemplateService, MessageTemplateService>();
        services.AddScoped<IQueuedEmailService, QueuedEmailService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<INewsLetterSubscriptionService, NewsLetterSubscriptionService>();
        services.AddScoped<INewsLetterSubscriptionTypeService, NewsLetterSubscriptionTypeService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IEmailAccountService, EmailAccountService>();
        services.AddScoped<ITokenizer, Tokenizer>();
        services.AddScoped<ISmtpBuilder, SmtpBuilder>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IAuthenticationService, CookieAuthenticationService>();
        services.AddScoped<IUrlRecordService, UrlRecordService>();
        services.AddScoped<ILogger, DefaultLogger>();
        services.AddScoped<ICustomerActivityService, CustomerActivityService>();
        services.AddScoped<IDateTimeHelper, DateTimeHelper>();
        services.AddScoped<INopHtmlHelper, NopHtmlHelper>();
        services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
        services.AddSingleton<IThemeProvider, ThemeProvider>();
        services.AddScoped<IThemeContext, ThemeContext>();
        services.AddSingleton<IRoutePublisher, RoutePublisher>();
        services.AddSingleton<IEventPublisher, EventPublisher>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IBBCodeHelper, BBCodeHelper>();
        services.AddScoped<IHtmlFormatter, HtmlFormatter>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<INopUrlHelper, NopUrlHelper>();
        services.AddScoped<IWidgetModelFactory, WidgetModelFactory>();
        services.AddScoped<IImportManager, ImportManager>();
        services.AddScoped<IExportManager, ExportManager>();


        //COMMERCE SERVICES REMOVED - Phase B
        //Removed: Catalog services, Order services, Discount services, Shipping services, Tax services, Payment services, Vendor services, Affiliate services, etc.

        //plugin managers
        services.AddScoped(typeof(IPluginManager<>), typeof(PluginManager<>));
        services.AddScoped<IWidgetPluginManager, WidgetPluginManager>();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        //register all settings
        var typeFinder = Singleton<ITypeFinder>.Instance;

        var settings = typeFinder.FindClassesOfType(typeof(ISettings), false).ToList();
        foreach (var setting in settings)
        {
            services.AddScoped(setting, serviceProvider =>
            {
                var storeId = DataSettingsManager.IsDatabaseInstalled()
                    ? serviceProvider.GetRequiredService<IStoreContext>().GetCurrentStore()?.Id ?? 0
                    : 0;

                return serviceProvider.GetRequiredService<ISettingService>().LoadSettingAsync(setting, storeId).Result;
            });
        }

        //picture thumb service
        services.AddScoped<IThumbService, ThumbService>();

        //picture service
        services.AddScoped<IPictureService, PictureService>();

        //roxy file manager
        services.AddScoped<IRoxyFilemanService, RoxyFilemanService>();
        services.AddScoped<IRoxyFilemanFileProvider, RoxyFilemanFileProvider>();


        //slug route transformer
        if (DataSettingsManager.IsDatabaseInstalled())
            services.AddScoped<SlugRouteTransformer>();

        //schedule tasks
        services.AddSingleton<ITaskScheduler, TaskScheduler>();
        services.AddTransient<IScheduleTaskRunner, ScheduleTaskRunner>();

        //event consumers
        var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
        foreach (var consumer in consumers)
            foreach (var findInterface in consumer.FindInterfaces((type, criteria) =>
                     {
                         var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                         return isMatch;
                     }, typeof(IConsumer<>)))
                services.AddScoped(findInterface, consumer);

        //admin menu
        services.AddScoped<IAdminMenu, AdminMenu>();

        //register the Lazy resolver for .Net IoC
        var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
        if (!useAutofac)
            services.AddScoped(typeof(Lazy<>), typeof(LazyInstance<>));
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
    public int Order => 2000;
}