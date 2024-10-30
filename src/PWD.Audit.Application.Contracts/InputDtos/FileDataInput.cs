using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.InputDtos
{
    public class FileDataInput
    {
        public int Id { get; set; }
        //public int EmployeeId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string OriginalDocumentName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
        public bool IsFileUploaded { get; set; }
    }
}
