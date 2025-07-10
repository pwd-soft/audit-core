using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PWD.Audit.Models
{
    public class Attachment : FullAuditedEntity<int>
    {
        public int ObjectionId { get; set; }
        public int? ResponseId { get; set; } = 0;
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public AttachmentType AttachmentType { get; set; } = AttachmentType.None;
        public string Path { get; set; }
        public long FileSize { get; set; }
        public bool IsFileUploaded { get; set; }
    }
}
