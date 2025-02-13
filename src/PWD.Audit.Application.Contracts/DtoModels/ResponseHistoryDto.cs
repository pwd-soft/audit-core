using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class ResponseHistoryDto
    {
        public int ObjectionId { get; set; }
        public ObjectionStatus ObjectionStatus { get; set; } = ObjectionStatus.None;
        public string Response { get; set; } = string.Empty;
        public string MonitorComment { get; set; } = string.Empty;
        public string MonitorUsername { get; set; } = string.Empty;
        public bool LockStatus { get; set; } = false;
    }
}
