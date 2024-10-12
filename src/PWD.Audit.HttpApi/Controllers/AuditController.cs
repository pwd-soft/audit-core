using PWD.Audit.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PWD.Audit.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class AuditController : AbpController
    {
        protected AuditController()
        {
            LocalizationResource = typeof(AuditResource);
        }
    }
}