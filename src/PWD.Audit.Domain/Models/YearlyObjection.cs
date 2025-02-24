using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PWD.Audit.Models
{
    public class YearlyObjection : FullAuditedEntity<int>
    {
        public Guid OfficeId { get; set; }
        public string OfficeCode { get; set; } = string.Empty;
        public int Year { get; set; }
        public int NumberOfObjections { get; set; }
    }
}
