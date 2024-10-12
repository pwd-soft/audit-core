using PWD.Audit.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace PWD.Audit.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AuditEntityFrameworkCoreDbMigrationsModule),
        typeof(AuditApplicationContractsModule)
        )]
    public class AuditDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
