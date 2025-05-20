using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class ResponseStateDto : FullAuditedEntityDto<int>
    {
        public int ResponseHistoryId { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsModified { get; set; } = false;
        public string Office { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public int PostingId { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }
}
