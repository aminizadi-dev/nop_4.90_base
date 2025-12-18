using Nop.Core.Domain.Customers;
using static Nop.Services.Security.StandardPermission;

namespace Nop.Services.Security;

/// <summary>
/// Default permission config manager
/// </summary>
public partial class DefaultPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        #region Security
        
        new ("Security. Enable Multi-factor authentication", StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName),
        new ("Access admin area", StandardPermission.Security.ACCESS_ADMIN_PANEL, nameof(StandardPermission.Security), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Customers
        
        new ("Admin area. Customers. View", StandardPermission.Customers.CUSTOMERS_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Create, edit, delete", StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Import and export", StandardPermission.Customers.CUSTOMERS_IMPORT_EXPORT, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customers. Allow impersonation", StandardPermission.Customers.CUSTOMERS_IMPERSONATION, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customer roles. View", StandardPermission.Customers.CUSTOMER_ROLES_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Customer roles. Create, edit, delete", StandardPermission.Customers.CUSTOMER_ROLES_CREATE_EDIT_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        //COMMERCE PERMISSIONS REMOVED - Phase B
        //Removed: VENDORS_VIEW, VENDORS_CREATE_EDIT_DELETE, GDPR_MANAGE
        new ("Admin area. Activity Log. View", StandardPermission.Customers.ACTIVITY_LOG_VIEW, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Activity Log. Delete", StandardPermission.Customers.ACTIVITY_LOG_DELETE, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Activity Log. Manage types", StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES, nameof(StandardPermission.Customers), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        //COMMERCE PERMISSIONS REMOVED - Phase B
        //Removed: Orders section (entire section)
        //Removed: Reports section (entire section)
        //Removed: Catalog section (entire section)
        //Removed: Promotions section (entire section)

        #region Content management
        
        //COMMERCE CONTENT PERMISSIONS REMOVED - Phase B
        //Removed: Topics, News, Blog, Polls, Forums, Menus (content management features)
        new ("Admin area. Message Templates. View", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Message Templates. Create, edit, delete", StandardPermission.ContentManagement.MESSAGE_TEMPLATES_CREATE_EDIT_DELETE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),

        #endregion

        #region Configuration
        
        new ("Admin area. Widgets. Manage", StandardPermission.Configuration.MANAGE_WIDGETS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Countries. Manage", StandardPermission.Configuration.MANAGE_COUNTRIES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Languages. Manage", StandardPermission.Configuration.MANAGE_LANGUAGES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Settings. Manage", StandardPermission.Configuration.MANAGE_SETTINGS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        //COMMERCE CONFIGURATION PERMISSIONS REMOVED - Phase B
        //Removed: MANAGE_PAYMENT_METHODS, MANAGE_TAX_SETTINGS, MANAGE_SHIPPING_SETTINGS
        new ("Admin area. External Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Multi-factor Authentication Methods. Manage", StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. ACL. Manage", StandardPermission.Configuration.MANAGE_ACL, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Email Accounts. Manage", StandardPermission.Configuration.MANAGE_EMAIL_ACCOUNTS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Stores. Manage", StandardPermission.Configuration.MANAGE_STORES, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Plugins. Manage", StandardPermission.Configuration.MANAGE_PLUGINS, nameof(StandardPermission.Configuration), NopCustomerDefaults.AdministratorsRoleName),
        
        #endregion

        #region System
        
        new ("Admin area. System Log. Manage", StandardPermission.System.MANAGE_SYSTEM_LOG, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Message Queue. Manage", StandardPermission.System.MANAGE_MESSAGE_QUEUE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Maintenance. Manage", StandardPermission.System.MANAGE_MAINTENANCE, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. HTML Editor. Manage pictures", StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Schedule Tasks. Manage", StandardPermission.System.MANAGE_SCHEDULE_TASKS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. App Settings. Manage", StandardPermission.System.MANAGE_APP_SETTINGS, nameof(StandardPermission.System), NopCustomerDefaults.AdministratorsRoleName),

        #endregion
        
        //COMMERCE PERMISSIONS REMOVED - Phase B
        //Removed: PublicStore section (entire section)
    };
}