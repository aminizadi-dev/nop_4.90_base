namespace Nop.Services.Security;

/// <summary>
/// Standard permission
/// </summary>
public partial class StandardPermission
{
    public partial class Customers
    {
        public const string CUSTOMERS_VIEW = $"{nameof(Customers)}.CustomersView";
        public const string CUSTOMERS_CREATE_EDIT_DELETE = $"{nameof(Customers)}.CustomersCreateEditDelete";
        public const string CUSTOMERS_IMPORT_EXPORT = $"{nameof(Customers)}.CustomersImportExport";
        public const string CUSTOMERS_IMPERSONATION = $"{nameof(Customers)}.CustomersImpersonation";
        public const string CUSTOMER_ROLES_VIEW = $"{nameof(Customers)}.CustomerRolesView";
        public const string CUSTOMER_ROLES_CREATE_EDIT_DELETE = $"{nameof(Customers)}.CustomerRolesCreateEditDelete";
        public const string ACTIVITY_LOG_VIEW = $"{nameof(Customers)}.ActivityLogView";
        public const string ACTIVITY_LOG_DELETE = $"{nameof(Customers)}.ActivityLogDelete";
        public const string ACTIVITY_LOG_MANAGE_TYPES = $"{nameof(Customers)}.ActivityLogManageTypes";
    }

    //COMMERCE PERMISSIONS REMOVED - Phase B
    //Removed: Orders, Reports, Catalog, Promotions classes

    public partial class ContentManagement
    {
        public const string MESSAGE_TEMPLATES_VIEW = $"{nameof(ContentManagement)}.MessageTemplatesView";
        public const string MESSAGE_TEMPLATES_CREATE_EDIT_DELETE = $"{nameof(ContentManagement)}.MessageTemplatesCreateEditDelete";
        //COMMERCE CONTENT PERMISSIONS REMOVED - Phase B
        //Removed: TOPICS, NEWS, BLOG, POLLS, FORUMS, MENU
    }

    public partial class Configuration
    {
        public const string MANAGE_WIDGETS = $"{nameof(Configuration)}.ManageWidgets";
        public const string MANAGE_COUNTRIES = $"{nameof(Configuration)}.ManageCountries";
        public const string MANAGE_LANGUAGES = $"{nameof(Configuration)}.ManageLanguages";
        public const string MANAGE_SETTINGS = $"{nameof(Configuration)}.ManageSettings";
        public const string MANAGE_EXTERNAL_AUTHENTICATION_METHODS = $"{nameof(Configuration)}.ManageExternalAuthenticationMethods";
        public const string MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS = $"{nameof(Configuration)}.ManageMultifactorAuthenticationMethods";
        public const string MANAGE_ACL = $"{nameof(Configuration)}.ManageACL";
        public const string MANAGE_EMAIL_ACCOUNTS = $"{nameof(Configuration)}.ManageEmailAccounts";
        public const string MANAGE_STORES = $"{nameof(Configuration)}.ManageStores";
        public const string MANAGE_PLUGINS = $"{nameof(Configuration)}.ManagePlugins";
        //COMMERCE CONFIGURATION PERMISSIONS REMOVED - Phase B
        //Removed: MANAGE_PAYMENT_METHODS, MANAGE_TAX_SETTINGS, MANAGE_SHIPPING_SETTINGS
    }

    public partial class System
    {
        public const string MANAGE_SYSTEM_LOG = $"{nameof(System)}.ManageSystemLog";
        public const string MANAGE_MESSAGE_QUEUE = $"{nameof(System)}.ManageMessageQueue";
        public const string MANAGE_MAINTENANCE = $"{nameof(System)}.ManageMaintenance";
        public const string HTML_EDITOR_MANAGE_PICTURES = $"{nameof(System)}.HtmlEditor.ManagePictures";
        public const string MANAGE_SCHEDULE_TASKS = $"{nameof(System)}.ManageScheduleTasks";
        public const string MANAGE_APP_SETTINGS = $"{nameof(System)}.ManageAppSettings";
    }

    public partial class Security
    {
        public const string ENABLE_MULTI_FACTOR_AUTHENTICATION = $"{nameof(Security)}.EnableMultiFactorAuthentication";
        public const string ACCESS_ADMIN_PANEL = $"{nameof(Security)}.AccessAdminPanel";
    }

    public partial class PublicStore
    {
        public const string PUBLIC_STORE_ALLOW_NAVIGATION = $"{nameof(PublicStore)}.PublicStoreAllowNavigation";
        public const string ACCESS_CLOSED_STORE = $"{nameof(PublicStore)}.AccessClosedStore";
    }
}