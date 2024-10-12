using Microsoft.AspNetCore.Authorization;
using PWDEstimate;
using System;

namespace PWD.Schedule
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionType permission)
           : base(permission.ToClaim()) { }
    }
}