using System;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class SummaryLine : FullAuditedEntity<int>
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
        public double Value { get; set; }
        public int BroadSheet{ get; set; }
        public int NonBroadSheet{ get; set; }
        public int Resolved{ get; set; }
        public int Note{ get; set; }
    }
}
