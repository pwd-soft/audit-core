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
        public ObjectionStatus ObjectionStatus { get; set; } = ObjectionStatus.None;
        public string Response { get; set; } = string.Empty;
        public string MonitorComment { get; set; } = string.Empty;
        public string MonitorUsername { get; set; } = string.Empty;
        public bool LockStatus { get; set; } = false;
    }
}
