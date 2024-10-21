using System;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class Associate:FullAuditedEntity<int>
    {
        public int ObjectionId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Post { get; set; }
        public string Note { get; set; }
    }
}
