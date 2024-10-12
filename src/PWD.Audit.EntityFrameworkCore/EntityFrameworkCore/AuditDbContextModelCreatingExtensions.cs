using Microsoft.EntityFrameworkCore;
using PWD.Audit.Entities;
using Volo.Abp;

namespace PWD.Audit.EntityFrameworkCore
{
    public static class AuditDbContextModelCreatingExtensions
    {
        public static void ConfigureAudit(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(AuditConsts.DbTablePrefix + "YourEntities", AuditConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});

            builder.Entity<Objection>(b => b.ToTable("Objections"));
            builder.Entity<Associate>(b => b.ToTable("Associates"));
            builder.Entity<OfficeUser>(b => b.ToTable("OfficeUsers"));
            builder.Entity<Summary>(b => b.ToTable("Summaries"));
            builder.Entity<SummaryLine>(b => b.ToTable("SummaryLines"));



        }
    }
}