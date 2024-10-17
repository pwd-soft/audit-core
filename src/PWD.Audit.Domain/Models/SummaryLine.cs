using PWD.Audit.Enum;
using System;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class SummaryLine : FullAuditedEntity<int>
    {
        public int SummaryId { get; set; }
        public ObjectionType Type { get; set; }        
        public int Count { get; set; }
        public double Value { get; set; }
        public int BroadSheet { get; set; }
        public int NonBroadSheet { get; set; }
        public int Resolved { get; set; }
        public string Note { get; set; }
    }
}
