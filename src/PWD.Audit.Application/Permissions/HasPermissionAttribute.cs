using Microsoft.AspNetCore.Authorization; 
using System;

namespace PWD.Audit
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionType permission)
           : base(permission.ToClaim()) { }
    }
}