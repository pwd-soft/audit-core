using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class Summary : FullAuditedEntity<int>
    {
        public string OfficeCode { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string ReferenceNo { get; set; }
        public int Note{ get; set; }
        public List<SummaryLine> SummaryLines { get; set; }=new List<SummaryLine>();
    }
}
