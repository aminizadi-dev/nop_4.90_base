using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Menu;

/// <summary>
/// Admin menu
/// </summary>
public partial class AdminMenu : IAdminMenu
{
    #region Fields

    protected AdminMenuItem _baseRootMenuItem;
    protected AdminMenuItem _rootItem;

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public AdminMenu(
        IActionContextAccessor actionContextAccessor,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IUrlHelperFactory urlHelperFactory,
        IWorkContext workContext)
    {
        _actionContextAccessor = actionContextAccessor;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _urlHelperFactory = urlHelperFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Fills the base root menu item data
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task FillBaseRootAsync()
    {
        if (_baseRootMenuItem != null)
            return;

        _baseRootMenuItem = new AdminMenuItem
        {
            SystemName = "Home",
            Title = await _localizationService.GetResourceAsync("Admin.Home"),
            Url = GetMenuItemUrl("Home", "Overview"),
            ChildNodes = new List<AdminMenuItem>
            {
                //dashboard
                new()
                {
                    SystemName = "Dashboard",
                    Title = await _localizationService.GetResourceAsync("Admin.Dashboard"),
                    Url = GetMenuItemUrl("Home", "Index"),
                    IconClass = "fas fa-desktop"
                },
                //customers
                new()
                {
                    SystemName = "Customers",
                    Title = await _localizationService.GetResourceAsync("Admin.Customers"),
                    IconClass = "far fa-user",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Customers list",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.Customers"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("Customer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Customer roles",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMER_ROLES_VIEW },
                            Url = GetMenuItemUrl("CustomerRole", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Online customers",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers"),
                            PermissionNames = new List<string> { StandardPermission.Customers.CUSTOMERS_VIEW },
                            Url = GetMenuItemUrl("OnlineCustomer", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity logs",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.ActivityLog"),
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityLogs"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Activity types",
                            Title = await _localizationService.GetResourceAsync("Admin.Customers.ActivityLogType"),
                            PermissionNames = new List<string> { StandardPermission.Customers.ACTIVITY_LOG_VIEW },
                            Url = GetMenuItemUrl("ActivityLog", "ActivityTypes"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //content management (infrastructure only - message templates)
                new()
                {
                    SystemName = "Content Management",
                    Title = await _localizationService.GetResourceAsync("Admin.ContentManagement"),
                    IconClass = "fas fa-cubes",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Message templates",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.MessageTemplates"),
                            PermissionNames =
                                new List<string>
                                {
                                    StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW
                                },
                            Url = GetMenuItemUrl("MessageTemplate", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //configuration
                new()
                {
                    SystemName = "Configuration",
                    Title = await _localizationService.GetResourceAsync("Admin.Configuration"),
                    IconClass = "fas fa-cogs",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Settings",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_SETTINGS },
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "General settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon"),
                                    Url = GetMenuItemUrl("Setting", "GeneralCommon"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Customer and user settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser"),
                                    Url = GetMenuItemUrl("Setting", "CustomerUser"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Media settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Media"),
                                    Url = GetMenuItemUrl("Setting", "Media"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "App settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.AppSettings"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.System.MANAGE_APP_SETTINGS
                                        },
                                    Url = GetMenuItemUrl("Setting", "AppSettings"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "All settings",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings"),
                                    Url = GetMenuItemUrl("Setting", "AllSettings"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        },
                        new()
                        {
                            SystemName = "Email accounts",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.EmailAccounts"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS },
                            Url = GetMenuItemUrl("EmailAccount",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Stores",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Stores"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_STORES },
                            Url = GetMenuItemUrl("Store",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Countries",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Countries"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_COUNTRIES },
                            Url = GetMenuItemUrl("Country",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Languages",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Languages"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_LANGUAGES },
                            Url = GetMenuItemUrl("Language",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Access control list",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.ACL"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_ACL },
                            Url = GetMenuItemUrl("Security", "Permissions"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Widgets",
                            Title = await _localizationService.GetResourceAsync("Admin.ContentManagement.Widgets"),
                            PermissionNames = new List<string> { StandardPermission.Configuration.MANAGE_WIDGETS },
                            Url = GetMenuItemUrl("Widget", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Authentication",
                            Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication"),
                            IconClass = "far fa-dot-circle",
                            ChildNodes = new List<AdminMenuItem>
                            {
                                new()
                                {
                                    SystemName = "External authentication methods",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication.ExternalMethods"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS
                                        },
                                    Url = GetMenuItemUrl("Authentication", "ExternalMethods"),
                                    IconClass = "far fa-circle"
                                },
                                new()
                                {
                                    SystemName = "Multi-factor authentication methods",
                                    Title = await _localizationService.GetResourceAsync("Admin.Configuration.Authentication.MultiFactorMethods"),
                                    PermissionNames =
                                        new List<string>
                                        {
                                            StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS
                                        },
                                    Url = GetMenuItemUrl("Authentication", "MultiFactorMethods"),
                                    IconClass = "far fa-circle"
                                }
                            }
                        }
                    }
                },
                //system
                new()
                {
                    SystemName = "System",
                    Title = await _localizationService.GetResourceAsync("Admin.System"),
                    IconClass = "fas fa-cube",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "System information",
                            Title = await _localizationService.GetResourceAsync("Admin.System.SystemInfo"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SystemInfo"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Log",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Log"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SYSTEM_LOG },
                            Url = GetMenuItemUrl("Log", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Warnings",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Warnings"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Warnings"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Maintenance",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Maintenance"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "Maintenance"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Queued emails",
                            Title =
                                await _localizationService.GetResourceAsync("Admin.System.QueuedEmails"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MESSAGE_QUEUE },
                            Url = GetMenuItemUrl("QueuedEmail", "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Schedule tasks",
                            Title = await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_SCHEDULE_TASKS },
                            Url = GetMenuItemUrl("ScheduleTask",
                            "List"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Search engine friendly names",
                            Title = await _localizationService.GetResourceAsync("Admin.System.SeNames"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Common", "SeNames"),
                            IconClass = "far fa-dot-circle"
                        },
                        new()
                        {
                            SystemName = "Templates",
                            Title = await _localizationService.GetResourceAsync("Admin.System.Templates"),
                            PermissionNames = new List<string> { StandardPermission.System.MANAGE_MAINTENANCE },
                            Url = GetMenuItemUrl("Template", "List"),
                            IconClass = "far fa-dot-circle"
                        }
                    }
                },
                //help
                new()
                {
                    SystemName = "Help",
                    Title = await _localizationService.GetResourceAsync("Admin.Help"),
                    IconClass = "fas fa-question-circle",
                    ChildNodes = new List<AdminMenuItem>
                    {
                        new()
                        {
                            SystemName = "Training",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Training"),
                            Url = "https://www.nopcommerce.com/training?utm_source=admin-panel&utm_medium=menu&utm_campaign=course&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Documentation",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Documentation"),
                            Url = "https://docs.nopcommerce.com?utm_source=admin-panel&utm_medium=menu&utm_campaign=documentation&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Community forums",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.Forums"),
                            Url = "https://www.nopcommerce.com/boards?utm_source=admin-panel&utm_medium=menu&utm_campaign=forum&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Premium support services",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.SupportServices"),
                            Url = "https://www.nopcommerce.com/nopcommerce-premium-support-services?utm_source=admin-panel&utm_medium=menu&utm_campaign=premium_support&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        },
                        new()
                        {
                            SystemName = "Solution partners",
                            Title = await _localizationService.GetResourceAsync("Admin.Help.SolutionPartners"),
                            Url = "https://www.nopcommerce.com/solution-partners?utm_source=admin-panel&utm_medium=menu&utm_campaign=solution_partners&utm_content=help",
                            IconClass = "far fa-dot-circle",
                            OpenUrlInNewTab = true
                        }
                    }
                }
            }
        };
    }

    /// <summary>
    /// Loads admin menu
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item for admin menu
    /// </returns>
    protected virtual async Task<AdminMenuItem> LoadMenuAsync(bool showHidden)
    {
        await FillBaseRootAsync();

        AdminMenuItem cloneMenuItem(AdminMenuItem item)
        {
            return new AdminMenuItem
            {
                PermissionNames = item.PermissionNames,
                ChildNodes = item.ChildNodes.Select(cloneMenuItem).ToList(),
                IconClass = item.IconClass,
                Visible = item.Visible,
                OpenUrlInNewTab = item.OpenUrlInNewTab,
                SystemName = item.SystemName,
                Title = item.Title,
                Url = item.Url
            };
        }

        var root = cloneMenuItem(_baseRootMenuItem);

        var customer = await _workContext.GetCurrentCustomerAsync();

        await _eventPublisher.PublishAsync(new AdminMenuCreatedEvent(this, root));

        async ValueTask<bool> authorizePermission(string permissionName) => await _permissionService.AuthorizeAsync(permissionName.Trim());

        async Task checkPermissions(AdminMenuItem menuItem, AdminMenuItem rootItem = null)
        {
            if (menuItem.Visible)
            {
                var permissions = (menuItem.PermissionNames.Any() ? menuItem.PermissionNames : (rootItem?.PermissionNames ?? new List<string>())).Distinct().Where(p => !string.IsNullOrEmpty(p)).ToList();

                if (permissions.Any())
                    menuItem.Visible = menuItem.ChildNodes.Any() ? await permissions.AnyAwaitAsync(authorizePermission) : await permissions.AllAwaitAsync(authorizePermission);
            }

            foreach (var childNode in menuItem.ChildNodes)
                await checkPermissions(childNode, menuItem);
        }

        await checkPermissions(root);

        if (showHidden)
            return root;

        void checkVisible(AdminMenuItem menuItem)
        {
            if (!menuItem.ChildNodes.Any())
            {
                menuItem.Visible = menuItem.Visible && !string.IsNullOrEmpty(menuItem.Url);

                return;
            }

            foreach (var childNode in menuItem.ChildNodes)
                checkVisible(childNode);

            menuItem.Visible = menuItem.ChildNodes.Any(n => n.Visible);
        }

        checkVisible(root);

        return root;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the root node
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item
    /// </returns>
    public virtual async Task<AdminMenuItem> GetRootNodeAsync(bool showHidden = false)
    {
        if (_rootItem != null)
            return _rootItem;

        _rootItem = await LoadMenuAsync(showHidden);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        void transformUrl(AdminMenuItem node)
        {
            if (node.Url?.StartsWith("~/", StringComparison.Ordinal) ?? false)
                node.Url = urlHelper.Content(node.Url);

            foreach (var childNode in node.ChildNodes)
                transformUrl(childNode);
        }

        transformUrl(_rootItem);

        return _rootItem;
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    /// <returns>Menu item URL</returns>
    public virtual string GetMenuItemUrl(string controllerName, string actionName)
    {
        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            return null;

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext ?? throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext)));

        return urlHelper.Action(actionName, controllerName, new RouteValueDictionary { { "area", AreaNames.ADMIN } }, null, null);
    }

    #endregion
}