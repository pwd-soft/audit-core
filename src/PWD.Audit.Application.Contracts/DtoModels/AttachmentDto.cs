using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class AttachmentDto : EntityDto<int>
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
