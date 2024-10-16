using System;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class OfficeUser : FullAuditedEntity<int>
    {
        public Guid OfficeId { get; set; }
        public Guid UserId { get; set; }
        public int PostingId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public bool IsActive { get; set; }=false;
    }
}
