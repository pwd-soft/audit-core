using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.InputDtos
{
    public class FileDataInput
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
        public bool IsFileUploaded { get; set; }
    }
}
