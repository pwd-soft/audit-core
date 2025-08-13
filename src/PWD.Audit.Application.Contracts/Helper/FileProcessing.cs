using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PWD.Audit.Helper
{
    public static class FileProcessing
    {
        public static void PorcessFilesToUploadFolder(int objectionId, List<AttachmentDto> fileDataInput)
        {
            var directoryName = objectionId.ToString();
            var folderName = Path.Combine("wwwroot", "Uploaded_Documents", directoryName);
            if (!Directory.Exists(folderName))
            {
                DirectoryInfo di = Directory.CreateDirectory(folderName);
            }

            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            foreach (var file in fileDataInput)
            {
                var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.Path);
                var destinationPath = Path.Combine(pathToSave, file.FileName);

                System.IO.File.Copy(sourcePath, destinationPath, true);
                System.IO.File.Delete(sourcePath);
                var savedFileName = file.Path.Split(@"\")[1];
                var path = Path.Combine(folderName, savedFileName);
                path = path.Replace(@"wwwroot\", string.Empty);

                file.Path = path;
            }
        }
    }
}
