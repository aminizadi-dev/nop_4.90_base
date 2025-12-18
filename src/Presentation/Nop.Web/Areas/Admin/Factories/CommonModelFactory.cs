using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
//COMMERCE DOMAIN/SERVICES REMOVED - Phase B
//Removed: using Nop.Core.Domain.Catalog;
//Removed: using Nop.Core.Domain.Orders;
//Removed: using Nop.Services.Blogs;
//Removed: using Nop.Services.Catalog;
//Removed: using Nop.Services.News;
//Removed: using Nop.Services.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
//COMMERCE MODELS REMOVED - Phase B
//Removed: using Nop.Web.Areas.Admin.Models.Blogs;
//Removed: using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Localization;
//Removed: using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Security;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents common models factory implementation
/// </summary>
public partial class CommonModelFactory : ICommonModelFactory
{
    #region Fields

    protected readonly AppSettings _appSettings;
    //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
    //Removed: protected readonly CatalogSettings _catalogSettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    //Removed: protected readonly IBlogService _blogService;
    //Removed: protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMaintenanceService _maintenanceService;
    //Removed: protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    //Removed: protected readonly INewsService _newsService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INopUrlHelper _nopUrlHelper;
    //COMMERCE SERVICES REMOVED - Phase B
    //Removed: protected readonly IOrderService _orderService;
    //Removed: protected readonly IProductService _productService;
    //Removed: protected readonly IReturnRequestService _returnRequestService;
    protected readonly ISearchTermService _searchTermService;
    protected readonly IServiceCollection _serviceCollection;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IThumbService _thumbService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly NopHttpClient _nopHttpClient;
    protected readonly ProxySettings _proxySettings;

    #endregion

    #region Ctor

    public CommonModelFactory(AppSettings appSettings,
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
        //Removed: CatalogSettings catalogSettings,
        IActionContextAccessor actionContextAccessor,
        IBaseAdminModelFactory baseAdminModelFactory,
        //Removed: IBlogService blogService,
        //Removed: ICategoryService categoryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMaintenanceService maintenanceService,
        //Removed: IManufacturerService manufacturerService,
        IMeasureService measureService,
        //Removed: INewsService newsService,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        INopUrlHelper nopUrlHelper,
        //COMMERCE SERVICES REMOVED - Phase B
        //Removed: IOrderService orderService,
        //Removed: IProductService productService,
        //Removed: IReturnRequestService returnRequestService,
        ISearchTermService searchTermService,
        IServiceCollection serviceCollection,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        IThumbService thumbService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        NopHttpClient nopHttpClient,
        ProxySettings proxySettings)
    {
        _appSettings = appSettings;
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
        //Removed: _catalogSettings = catalogSettings;
        _actionContextAccessor = actionContextAccessor;
        _baseAdminModelFactory = baseAdminModelFactory;
        //Removed: _blogService = blogService;
        //Removed: _categoryService = categoryService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _dataProvider = dataProvider;
        _dateTimeHelper = dateTimeHelper;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _maintenanceService = maintenanceService;
        //Removed: _manufacturerService = manufacturerService;
        _measureService = measureService;
        //Removed: _newsService = newsService;
        _fileProvider = fileProvider;
        _nopUrlHelper = nopUrlHelper;
        //COMMERCE SERVICES REMOVED - Phase B
        //Removed: _orderService = orderService;
        //Removed: _productService = productService;
        //Removed: _returnRequestService = returnRequestService;
        _searchTermService = searchTermService;
        _serviceCollection = serviceCollection;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _thumbService = thumbService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _nopHttpClient = nopHttpClient;
        _proxySettings = proxySettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare store URL warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareStoreUrlWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //check whether current store URL matches the store configured URL
        var store = await _storeContext.GetCurrentStoreAsync();
        var currentStoreUrl = store.Url;
        if (!string.IsNullOrEmpty(currentStoreUrl) &&
            (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
             currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)))
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.Match")
            });
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Fail,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.NoMatch"),
                currentStoreUrl, _webHelper.GetStoreLocation(false))
        });
    }

    /// <summary>
    /// Prepare recommendations/warnings model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareRecommendationsModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        var recommendations = new List<string>();
        try
        {
            var text = await _nopHttpClient.GetRecommendationsAsync();
            recommendations = JsonConvert.DeserializeObject<List<string>>(text);
        }
        catch { }

        foreach (var recommendation in recommendations ?? new())
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Recommendation,
                Text = recommendation,
                DontEncode = true
            });
        }
    }

    /// <summary>
    /// Prepare base weight warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareBaseWeightWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //check whether base measure weight set
        var baseWeight = await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
        if (baseWeight == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.NotSet")
            });
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Set")
        });

        //check whether base measure weight ratio configured
        if (baseWeight.Ratio != 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Ratio1")
            });
        }
    }

    /// <summary>
    /// Prepare base dimension warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareBaseDimensionWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //check whether base measure dimension set
        var baseDimension = await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
        if (baseDimension == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.NotSet")
            });
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Set")
        });

        //check whether base measure dimension ratio configured
        if (baseDimension.Ratio != 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Ratio1")
            });
        }
    }

    /// <summary>
    /// Prepare performance settings warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePerformanceSettingsWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //COMMERCE SETTINGS CHECKS REMOVED - Phase B
        //Removed: CatalogSettings.IgnoreStoreLimitations check
        //Removed: CatalogSettings.IgnoreAcl check
    }

    /// <summary>
    /// Prepare file permissions warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareFilePermissionsWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        var dirPermissionsOk = true;
        var dirsToCheck = _fileProvider.GetDirectoriesWrite();
        foreach (var dir in dirsToCheck)
        {
            if (_fileProvider.CheckPermissions(dir, false, true, true, false))
                continue;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.Wrong"),
                    CurrentOSUser.FullName, dir)
            });
            dirPermissionsOk = false;
        }

        if (dirPermissionsOk)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.OK")
            });
        }

        var filePermissionsOk = true;
        var filesToCheck = _fileProvider.GetFilesWrite();
        foreach (var file in filesToCheck)
        {
            if (_fileProvider.CheckPermissions(file, false, true, true, true))
                continue;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.Wrong"),
                    CurrentOSUser.FullName, file)
            });
            filePermissionsOk = false;
        }

        if (filePermissionsOk)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.OK")
            });
        }
    }

    /// <summary>
    /// Prepare backup file search model
    /// </summary>
    /// <param name="searchModel">Backup file search model</param>
    /// <returns>Backup file search model</returns>
    protected virtual BackupFileSearchModel PrepareBackupFileSearchModel(BackupFileSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare delete thumb files model
    /// </summary>
    /// <param name="model">Delete thumb files model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareDeleteThumbFilesModelAsync(MaintenanceModel.DeleteThumbFilesModel model)
    {
        if (_thumbService is not ThumbService thumbService)
        {
            model.IsDeleteThumbsSupported = false;
            return;
        }

        var (filesCount, filesSize) = await thumbService.GetThumbsInfoAsync();

        model.IsDeleteThumbsSupported = true;
        model.FilesCountText = string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.DeleteThumbFiles.FilesCount"), filesCount);
        model.FilesSizeText = string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.DeleteThumbFiles.FilesSize"), Math.Round(filesSize / 1024M / 1024M, 2));
    }

    /// <summary>
    /// Prepare plugins which try to override the same interface warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePluginsOverrideSameInterfaceWarningModelAsync(IList<SystemWarningModel> models)
    {
        //check whether there are different plugins which try to override the same interface
        var baseLibraries = new[] { "Nop.Core", "Nop.Data", "Nop.Services", "Nop.Web", "Nop.Web.Framework" };
        var overridenServices = _serviceCollection.Where(p =>
                p.ServiceType.FullName != null &&
                p.ServiceType.FullName.StartsWith("Nop.", StringComparison.InvariantCulture) &&
                !p.ServiceType.FullName.StartsWith(
                    typeof(IConsumer<>).FullName?.Replace("~1", string.Empty) ?? string.Empty,
                    StringComparison.InvariantCulture)).Select(p =>
                KeyValuePair.Create(p.ServiceType.FullName, p.ImplementationType?.Assembly.GetName().Name))
            .Where(p => baseLibraries.All(library =>
                !p.Value?.StartsWith(library, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .GroupBy(p => p.Key, p => p.Value)
            .Where(p => p.Count() > 1)
            .ToDictionary(p => p.Key, p => p.ToList());

        foreach (var overridenService in overridenServices)
        {
            var assemblies = overridenService.Value
                .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginsOverrideSameService"), overridenService.Key, assemblies)
            });
        }
    }

    /// <summary>
    /// Prepare plugins collision of loaded assembly warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task PreparePluginsCollisionsWarningModelAsync(IList<SystemWarningModel> models)
    {
        // Plugin functionality removed
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare incompatible plugins warning model 
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task PrepareIncompatibleWarningModelAsync(IList<SystemWarningModel> models)
    {
        // Plugin functionality removed
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare plugins installed warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task PreparePluginsInstalledWarningModelAsync(IList<SystemWarningModel> models)
    {
        // Plugin functionality removed
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare plugins enabled warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task PreparePluginsEnabledWarningModelAsync(IList<SystemWarningModel> models)
    {
        // Plugin functionality removed
        return Task.CompletedTask;
    }

    /// <summary>
    /// Prepare multistore preview models for an entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of multistore preview models for an entity
    /// </returns>
    protected virtual async Task<IList<MultistorePreviewModel>> PrepareMultistorePreviewModelsForEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, ISlugSupported
    {
        var models = new List<MultistorePreviewModel>();
        var stores = await _storeService.GetAllStoresAsync();

        foreach (var store in stores)
        {
            if (!Uri.TryCreate(store.Url, UriKind.Absolute, out var url))
                continue;

            models.Add(new MultistorePreviewModel
            {
                StoreName = store.Name,
                Url = await _nopUrlHelper
                    .RouteGenericUrlAsync(entity, url.Scheme, url.IsDefaultPort ? url.Host : $"{url.Host}:{url.Port}", null, null, false),
            });
        }

        return models;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare system info model
    /// </summary>
    /// <param name="model">System info model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the system info model
    /// </returns>
    public virtual async Task<SystemInfoModel> PrepareSystemInfoModelAsync(SystemInfoModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.NopVersion = NopVersion.FULL_VERSION;
        model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
        model.ServerLocalTime = DateTime.Now;
        model.UtcTime = DateTime.UtcNow;
        model.CurrentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
        model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

        //ensure no exception is thrown
        try
        {
            model.DatabaseCollation = await _dataProvider.GetDataBaseCollationAsync();
            model.OperatingSystem = Environment.OSVersion.VersionString;
            model.AspNetInfo = RuntimeInformation.FrameworkDescription;
            model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted;
        }
        catch
        {
            // ignored
        }

        foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
        {
            if (header.Key != HeaderNames.Cookie)
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var loadedAssemblyModel = new SystemInfoModel.LoadedAssembly
            {
                FullName = assembly.FullName
            };

            //ensure no exception is thrown
            try
            {
                loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
                loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                    .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

                //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
                //we use a simple method because the more Jeff Atwood's solution doesn't work anymore 
                //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
                loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(_fileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

            }
            catch
            {
                // ignored
            }

            model.LoadedAssemblies.Add(loadedAssemblyModel);
        }

        var currentStaticCacheManagerName = _staticCacheManager.GetType().Name;

        if (_appSettings.Get<DistributedCacheConfig>().Enabled)
            currentStaticCacheManagerName +=
                $"({await _localizationService.GetLocalizedEnumAsync(_appSettings.Get<DistributedCacheConfig>().DistributedCacheType)})";

        model.CurrentStaticCacheManager = currentStaticCacheManagerName;

        return model;
    }

    /// <summary>
    /// Prepare proxy connection warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProxyConnectionWarningModelAsync(IList<SystemWarningModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //whether proxy is enabled
        if (!_proxySettings.Enabled)
            return;

        try
        {
            await _nopHttpClient.PingAsync();

            //connection is OK
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.OK")
            });
        }
        catch
        {
            //connection failed
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.Failed")
            });
        }
    }

    /// <summary>
    /// Prepare system warning models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of system warning models
    /// </returns>
    public virtual async Task<IList<SystemWarningModel>> PrepareSystemWarningModelsAsync()
    {
        var models = new List<SystemWarningModel>();

        //store URL
        await PrepareStoreUrlWarningModelAsync(models);

        //recommendations
        await PrepareRecommendationsModelAsync(models);

        //base measure weight
        await PrepareBaseWeightWarningModelAsync(models);

        //base dimension weight
        await PrepareBaseDimensionWarningModelAsync(models);

        //performance settings
        await PreparePerformanceSettingsWarningModelAsync(models);

        //validate write permissions (the same procedure like during installation)
        await PrepareFilePermissionsWarningModelAsync(models);

        //proxy connection
        await PrepareProxyConnectionWarningModelAsync(models);

        //publish event and add another warnings (for example from plugins) 
        var warningEvent = new SystemWarningCreatedEvent();
        await _eventPublisher.PublishAsync(warningEvent);
        models.AddRange(warningEvent.SystemWarnings);

        return models;
    }

    /// <summary>
    /// Prepare maintenance model
    /// </summary>
    /// <param name="model">Maintenance model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the maintenance model
    /// </returns>
    public virtual async Task<MaintenanceModel> PrepareMaintenanceModelAsync(MaintenanceModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
        model.DeleteGuests.OnlyWithoutShoppingCart = true;
        model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);

        model.DeleteAlreadySentQueuedEmails.EndDate = DateTime.UtcNow.AddDays(-7);

        model.BackupSupported = _dataProvider.BackupSupported;

        //prepare nested search model
        PrepareBackupFileSearchModel(model.BackupFileSearchModel);

        //prepare nested DeleteThumbsFiles model
        await PrepareDeleteThumbFilesModelAsync(model.DeleteThumbsFiles);

        return model;
    }

    /// <summary>
    /// Prepare paged backup file list model
    /// </summary>
    /// <param name="searchModel">Backup file search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the backup file list model
    /// </returns>
    public virtual Task<BackupFileListModel> PrepareBackupFileListModelAsync(BackupFileSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get backup files
        var backupFiles = _maintenanceService.GetAllBackupFiles().ToPagedList(searchModel);

        //prepare list model
        var model = new BackupFileListModel().PrepareToGrid(searchModel, backupFiles, () =>
        {
            return backupFiles.Select(file => new BackupFileModel
            {
                Name = _fileProvider.GetFileName(file),

                //fill in additional values (not existing in the entity)
                Length = $"{_fileProvider.FileLength(file) / 1024f / 1024f:F2} Mb",

                Link = $"{_webHelper.GetStoreLocation()}db_backups/{_fileProvider.GetFileName(file)}"
            });
        });

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare URL record search model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record search model
    /// </returns>
    public virtual async Task<UrlRecordSearchModel> PrepareUrlRecordSearchModelAsync(UrlRecordSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available languages
        //we insert 0 as 'Standard' language.
        //let's insert -1 for 'All' language selection.
        await _baseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.System.SeNames.List.Language.Standard"));
        searchModel.AvailableLanguages.Insert(0,
            new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "-1" });
        searchModel.LanguageId = -1;

        //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.All")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.ActiveOnly")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.InactiveOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged URL record list model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record list model
    /// </returns>
    public virtual async Task<UrlRecordListModel> PrepareUrlRecordListModelAsync(UrlRecordSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
        var languageId = searchModel.LanguageId < 0 ? null : (int?)(searchModel.LanguageId);

        //get URL records
        var urlRecords = await _urlRecordService.GetAllUrlRecordsAsync(slug: searchModel.SeName,
            languageId: languageId, isActive: isActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //get URL helper
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        //prepare list model
        var model = await new UrlRecordListModel().PrepareToGridAsync(searchModel, urlRecords, () =>
        {
            return urlRecords.SelectAwait(async urlRecord =>
            {
                //fill in model values from the entity
                var urlRecordModel = urlRecord.ToModel<UrlRecordModel>();

                //fill in additional values (not existing in the entity)
                urlRecordModel.Name = urlRecord.Slug;
                urlRecordModel.Language = urlRecord.LanguageId == 0
                    ? await _localizationService.GetResourceAsync("Admin.System.SeNames.Language.Standard")
                    : (await _languageService.GetLanguageByIdAsync(urlRecord.LanguageId))?.Name ?? "Unknown";

                //details URL
                var detailsUrl = string.Empty;
                var entityName = urlRecord.EntityName?.ToLowerInvariant() ?? string.Empty;
                switch (entityName)
                {
                    //COMMERCE ENTITY TYPES REMOVED - Phase B
                    //Removed: blogpost, category, manufacturer, product, newsitem, vendor
                    case "topic":
                        detailsUrl = urlHelper.Action("Edit", "Topic", new { id = urlRecord.EntityId });
                        break;
                }

                urlRecordModel.DetailsUrl = detailsUrl;

                return urlRecordModel;
            });
        });
        return model;
    }

    /// <summary>
    /// Prepare language selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language selector model
    /// </returns>
    public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var model = new LanguageSelectorModel
        {
            CurrentLanguage = (await _workContext.GetWorkingLanguageAsync()).ToModel<LanguageModel>(),
            AvailableLanguages = (await _languageService
                    .GetAllLanguagesAsync(storeId: store.Id))
                .Select(language => language.ToModel<LanguageModel>()).ToList()
        };

        return model;
    }

    /// <summary>
    /// Prepare popular search term search model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term search model
    /// </returns>
    public virtual Task<PopularSearchTermSearchModel> PreparePopularSearchTermSearchModelAsync(PopularSearchTermSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize(5);

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged popular search term list model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term list model
    /// </returns>
    public virtual async Task<PopularSearchTermListModel> PreparePopularSearchTermListModelAsync(PopularSearchTermSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get popular search terms
        var searchTermRecordLines = await _searchTermService.GetStatsAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new PopularSearchTermListModel().PrepareToGrid(searchModel, searchTermRecordLines, () =>
        {
            return searchTermRecordLines.Select(searchTerm => new PopularSearchTermModel
            {
                Keyword = searchTerm.Keyword,
                Count = searchTerm.Count
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare common statistics model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the common statistics model
    /// </returns>
    public virtual async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
    {
        var model = new CommonStatisticsModel();

        //COMMERCE STATISTICS REMOVED - Phase B
        //Removed: NumberOfOrders, NumberOfPendingReturnRequests, NumberOfLowStockProducts

        var customerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
        model.NumberOfCustomers = (await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds,
            pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

        return model;
    }

    #endregion
}