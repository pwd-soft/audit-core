namespace PWD.Schedule
{
    //public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    //{
    //    private readonly IPermissionAppService _permissionAppService;
    //    public PermissionHandler(IPermissionAppService permissionAppService)
    //    {
    //        _permissionAppService = permissionAppService;
    //    }

    //    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
    //                                                   PermissionRequirement requirement)
    //    {
    //        var permissionsClaim = context.User.Claims
    //            .SingleOrDefault(c =>
    //                 c.Type == PermissionConstants.PackedPermissionClaimType);
    //        // If user does not have the scope claim, get out of here
    //        if (permissionsClaim == null)
    //            return Task.CompletedTask;

    //        if (permissionsClaim.Value
    //            .ThisPermissionIsAllowed(requirement.PermissionName))
    //            context.Succeed(requirement);

    //        return Task.CompletedTask;
    //    }
    //}
}