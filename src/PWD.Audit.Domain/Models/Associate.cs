using System;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class Associate:FullAuditedEntity<int>
    {
        public int ComplainId { get; set; }
        public int EmployeeId { get; set; }
        public int PostingId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Post { get; set; }
        public DateTime EngagedFrom { get; set; }
        public DateTime EngagedTo { get; set; }
        public string Note { get; set; }
    }
}
