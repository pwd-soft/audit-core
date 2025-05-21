using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace PWD.Audit.Models
{
    public class ResponseState : FullAuditedEntity<int>
    {
        public int ObjectionId { get; set; }
        public int? ResponseHistoryId { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsModified { get; set; } = false;
        public string Office { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public int PostingId { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }
}
