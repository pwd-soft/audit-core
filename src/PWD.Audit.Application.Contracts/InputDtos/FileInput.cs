using System;

namespace PWD.Audit.InputDtos
{
    public class FileInput
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsFileUploaded { get; set; }
    }
}
