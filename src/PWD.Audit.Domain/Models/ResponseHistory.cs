using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PWD.Audit.Models
{
    public class ResponseHistory : FullAuditedEntity<int>
    {
        public int ObjectionId { get; set; }
        public string Response { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public string User { get; set; }
        public ResponseStatus Status { get; set; } = ResponseStatus.None;

        public virtual ICollection<ResponseComment> ResponseComments { get; set; } = new List<ResponseComment>();
        public virtual ICollection<ResponseState> ResponseStates { get; set; } = new List<ResponseState>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
