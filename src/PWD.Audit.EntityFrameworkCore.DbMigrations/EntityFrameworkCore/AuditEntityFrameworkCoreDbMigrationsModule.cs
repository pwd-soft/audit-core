using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace PWD.Audit.EntityFrameworkCore
{
    [DependsOn(
        typeof(AuditEntityFrameworkCoreModule)
        )]
    public class AuditEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AuditMigrationsDbContext>();
        }
    }
}
