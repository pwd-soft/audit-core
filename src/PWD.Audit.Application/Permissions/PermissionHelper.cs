﻿namespace PWD.Schedule
{
    public static class PermissionHelper
    {
        public static readonly string _authority = "https://auth.mis1pwd.com";
        //public static readonly string _authority = "https://localhost:44380";
        public static readonly string _authorityDev = "https://localhost:44380";
        public static readonly string _identityClientUrl = "https://localhost:44329/";
        //public static readonly string _identityClientUrl = "https://localhost:44392";
        public static readonly string _identityClientUrlDev = "https://localhost:44392";
        public static readonly string _identityApiName = "/api/app/permission-map";
        public static readonly string _permissionGroupKey = "permissionGroupKey";
        public static readonly string _permissionGroupValue = "PWDAttendance";
        public static readonly string _providerName = "providerName";
        public static readonly string _providerValue = "R";
        public static readonly string _providerKey = "providerKeys";
        public static readonly string _clientId = "Attendance_App";
        public static readonly string _clientSecret = "1q2w3e*";
        public static readonly string _scope = "Attendance";
    }
}
