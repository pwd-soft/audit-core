using System;

namespace PWD.Audit
{
    public enum PermissionType
    {
        None = 0,
    }

    public static class PermissionTypeExtension
    {
        public static string ToClaim(this PermissionType permissionType)
        {
            string strPermissionType = permissionType switch
            {
                
                _ => throw new NotImplementedException($"Permission type - '{permissionType}' is not implemented."),
            };
            return strPermissionType;
        }
    }
}
