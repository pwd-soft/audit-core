using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;

namespace PWD.Schedule
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }
            var permissionss = context.User.Claims.Where(x => x.Type == "Permission" &&
                                                            x.Value == requirement.PermissionName &&
                                                            x.Issuer == "LOCAL AUTHORITY");
            if (permissionss.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}