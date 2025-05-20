using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class ResponseCommentDto : FullAuditedEntityDto<int>
    {
        public int ResponseHistoryId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Office { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public int PostingId { get; set; } = 0;
    }
}
