using PWD.Audit.Enum;
using PWD.Audit.InputDtos;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class ResponseHistoryDto: FullAuditedEntityDto<int>
    {
        public int ObjectionId { get; set; }
        public string Response { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public List<FileDataInput> FileDataInput { get; set; }
        public string Attachments { get; set; }
        public string User { get; set; }
        public ResponseStatus Status { get; set; }

        public virtual ICollection<ResponseCommentDto> ResponseComments { get; set; } = new List<ResponseCommentDto>();
        public virtual ICollection<ResponseStateDto> ResponseStates { get; set; } = new List<ResponseStateDto>();
    }
}
