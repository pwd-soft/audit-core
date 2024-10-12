using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.Auditing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace PWD.Audit
{
    [DependsOn(
        typeof(AuditDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(AuditApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
        )]
    public class AuditApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AuditApplicationModule>();
            });

            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = false; //Disables the auditing system
            });

            //  context.Services.Replace(
            //        ServiceDescriptor.Transient<
            //            IAuditingStore,
            //            EstimateAuditingLogStroe
            //>());
        }
    }
}
