using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class AuthenticationController : BaseAdminController
{
    #region Fields

    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

    #endregion

    #region Ctor

    public AuthenticationController(ExternalAuthenticationSettings externalAuthenticationSettings,
        IEventPublisher eventPublisher,
        IPermissionService permissionService,
        ISettingService settingService,
        MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
    {
        _externalAuthenticationSettings = externalAuthenticationSettings;
        _eventPublisher = eventPublisher;
        _permissionService = permissionService;
        _settingService = settingService;
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
    }

    #endregion

}