using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class ResponseHistoryDto: FullAuditedEntityDto<int>
    {
        public int ObjectionId { get; set; }
        public Guid OfficeId { get; set; }    
        public ObjectionStatus ObjectionStatus { get; set; } = ObjectionStatus.None;
        public string Response { get; set; } = string.Empty;
        public string MonitorComment { get; set; } = string.Empty;
        public string MonitorUsername { get; set; } = string.Empty;
        public bool LockStatus { get; set; } = false;
    }
}
