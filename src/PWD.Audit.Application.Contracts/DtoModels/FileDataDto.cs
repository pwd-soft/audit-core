using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class FileDataDto : EntityDto<int>
    {
        //public int EmployeeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string OriginalDocumentName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
    }
}
