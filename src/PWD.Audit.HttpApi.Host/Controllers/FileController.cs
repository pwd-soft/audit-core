using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System;
using PWD.Audit.DtoModels;
using System.Net.Http.Headers;
using PWD.Audit.InputDtos;
using Volo.Abp.AspNetCore.Mvc;

namespace PWD.Audit.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : AbpController
    {
        private readonly IWebHostEnvironment hostEnvironment;
        //private readonly IEmployeeAppService employeeAppService;
        //private readonly INomineeAppService nomineeAppService;

        public FileController(IWebHostEnvironment hostEnvironment
            //, 
            //IEmployeeAppService employeeAppService, INomineeAppService nomineeAppService
            )
        {
            this.hostEnvironment = hostEnvironment;
            //this.employeeAppService = employeeAppService;
            //this.nomineeAppService = nomineeAppService;
        }

        [HttpGet, ActionName("FileInputTest")]
        public FileInput FileInputTest() 
        {
            return new FileInput();
        }

        [HttpGet, ActionName("FileDataInputTest")]
        public FileDataInput FileDataInputTest() 
        {
            return new FileDataInput();
        }


        [HttpPost, ActionName("Upload")]
        [DisableRequestSizeLimit]
        public IActionResult FileUpload()
        {
            try
            {
                var files = Request.Form.Files;

                if (files.Count > 0)
                {
                    var attachments = new List<FileInput>();
                    foreach (var file in files)
                    {
                        //var directoryName = Request.Form["directoryName"][0];
                        //var folderName = Path.Combine("wwwroot", "Temp_Uploads", directoryName);
                        //if (!Directory.Exists(folderName))
                        //{
                        //    DirectoryInfo di = Directory.CreateDirectory(folderName);
                        //}

                        //var folderName = Path.Combine("wwwroot", "Uploaded_Documents");
                        var folderName = Path.Combine("wwwroot", "Temp_Uploads");

                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        var fileUniqueId = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);
                        var uniqueFileName = $"{fileUniqueId}_{fileName}";

                        var fullPath = Path.Combine(pathToSave, uniqueFileName);
                        var path = Path.Combine(folderName, uniqueFileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        path = path.Replace(@"wwwroot\", string.Empty);


                        attachments.Add(new FileInput { OriginalFileName = fileName, Path = path, FileSize = file.Length, IsFileUploaded = true });
                    }

                    var result = new
                    {
                        progress = 100,
                        files = attachments
                    };
                    return new JsonResult(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost, ActionName("DeleteFromFile")]
        public IActionResult DeleteFromFile(FileDeleteInput input)
        {
            try
            {
                var filePath = Path.Combine(this.hostEnvironment.WebRootPath, input.FilePath);
                FileInfo fi = new FileInfo(filePath);
                if (fi != null)
                {
                    System.IO.File.Delete(filePath);
                    fi.Delete();
                }
                return new JsonResult(input.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        [HttpPost, ActionName("DeleteFiles")]
        public IActionResult DeleteFiles(List<FileDeleteInput> deleteInputs)
        {
            try
            {
                var fileNames = new List<string>();
                foreach (var item in deleteInputs)
                {
                    var filePath = Path.Combine(this.hostEnvironment.WebRootPath, item.FilePath);
                    FileInfo fi = new FileInfo(filePath);
                    if (fi != null)
                    {
                        System.IO.File.Delete(filePath);
                        fi.Delete();
                    }
                    fileNames.Add(item.FileName);
                }
                return new JsonResult(fileNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        //[HttpGet, DisableRequestSizeLimit]
        //[ActionName("DownloadDocumentWithData")]
        //public async Task<IActionResult> DownloadDocument([FromQuery] string fileUrl, string fileName, int employeeId)
        //{
        //    //string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates\\পেনশন ফরম ২.১.docx");
        //    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl);
        //    string tempFilePath = Path.GetTempFileName(); // Generates a unique temporary file

        //    FileStream tempFileStream = null;

        //    try
        //    {
        //        using (FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        //        {
        //            tempFileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite);

        //            // Copy template to temporary file
        //            templateStream.CopyTo(tempFileStream);
        //            tempFileStream.Position = 0;

        //            using (WordprocessingDocument doc = WordprocessingDocument.Open(tempFileStream, true))
        //            {
        //                var data = await employeeAppService.GetEmployeeDetailAsync(employeeId);

        //                if (PensionFormNames.Pension_Form == fileName || PensionFormNames.Declaration_of_Valid_Inheritance == fileName)
        //                {

        //                    var nomineeData = data.Nominee;

        //                    // Replace placeholders
        //                    var replacements = new Dictionary<string, string>
        //                    {
        //                        {"{name}", data.Employee?.Name},
        //                        {"{NID}", data.Employee?.NID},
        //                        {"office", data.Employee?.Office},
        //                        {"lastpost", data.Employee?.CurrentDesignation},
        //                        { "{PresentAddress}",  data.Employee?.PresentAddress },
        //                        {"PermanentAddress",  data.Employee?.PermanentAddress},

        //                        {"father",  data.Employee?.FatherName },
        //                        {"{Mother}",  data.Employee?.MotherName },
        //                        { "{Nationality}",  data.Employee?.Nationality},
        //                        {"{dob}",  DateBn(data.Employee?.DateOfBirth) },
        //                        { "{JoinDate}",  DateBn(data.Employment?.JoiningDate) },
        //                        { "{RetireDate}", DateBn(data.Employment?.DateOfRetirement)},
        //                        { "{PensionType}",  data.SalaryAllowance?.NatureOfPension},

        //                        { "BankName",  data.Bank?.BankName },
        //                        { "BankBranch",  data.Bank?.BankBranch },
        //                        { "AccountNo",  data.Bank?.AccountNo },


        //                    };

        //                    if (nomineeData?.Count > 0)
        //                    {
        //                        replacements.Add("{n1_name}", nomineeData.ElementAtOrDefault(0).Name);
        //                        replacements.Add("{n1_dob}", DateBn(nomineeData.ElementAtOrDefault(0).DateOfBirth));
        //                        replacements.Add("{n1_relation}", nomineeData.ElementAtOrDefault(0).Relation);
        //                        replacements.Add("{n1_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(0).NominalRate));
        //                        replacements.Add("{n1_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(0).MaritalStatus));
        //                        replacements.Add("{n1_disability}", nomineeData.ElementAtOrDefault(0).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n1_name}", string.Empty);
        //                        replacements.Add("{n1_dob}", string.Empty);
        //                        replacements.Add("{n1_relation}", string.Empty);
        //                        replacements.Add("{n1_rate}", string.Empty);
        //                        replacements.Add("{n1_mStatus}", string.Empty);
        //                        replacements.Add("{n1_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 1)
        //                    {
        //                        replacements.Add("{n2_name}", nomineeData.ElementAtOrDefault(1).Name);
        //                        replacements.Add("{n2_dob}", DateBn(nomineeData.ElementAtOrDefault(1).DateOfBirth));
        //                        replacements.Add("{n2_relation}", nomineeData.ElementAtOrDefault(1).Relation);
        //                        replacements.Add("{n2_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(1).NominalRate));
        //                        replacements.Add("{n2_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(1).MaritalStatus));
        //                        replacements.Add("{n2_disability}", nomineeData.ElementAtOrDefault(1).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n2_name}", string.Empty);
        //                        replacements.Add("{n2_dob}", string.Empty);
        //                        replacements.Add("{n2_relation}", string.Empty);
        //                        replacements.Add("{n2_rate}", string.Empty);
        //                        replacements.Add("{n2_mStatus}", string.Empty);
        //                        replacements.Add("{n2_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 2)
        //                    {
        //                        replacements.Add("{n3_name}", nomineeData.ElementAtOrDefault(2).Name);
        //                        replacements.Add("{n3_dob}", DateBn(nomineeData.ElementAtOrDefault(2).DateOfBirth));
        //                        replacements.Add("{n3_relation}", nomineeData.ElementAtOrDefault(2).Relation);
        //                        replacements.Add("{n3_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(2).NominalRate));
        //                        replacements.Add("{n3_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(2).MaritalStatus));
        //                        replacements.Add("{n3_disability}", nomineeData.ElementAtOrDefault(2).Disability);

        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n3_name}", string.Empty);
        //                        replacements.Add("{n3_dob}", string.Empty);
        //                        replacements.Add("{n3_relation}", string.Empty);
        //                        replacements.Add("{n3_rate}", string.Empty);
        //                        replacements.Add("{n3_mStatus}", string.Empty);
        //                        replacements.Add("{n3_disability}", string.Empty);
        //                    }
        //                    if (nomineeData?.Count > 3)
        //                    {
        //                        replacements.Add("{n4_name}", nomineeData.ElementAtOrDefault(3).Name);
        //                        replacements.Add("{n4_dob}", DateBn(nomineeData.ElementAtOrDefault(3).DateOfBirth));
        //                        replacements.Add("{n4_relation}", nomineeData.ElementAtOrDefault(3).Relation);
        //                        replacements.Add("{n4_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(3).NominalRate));
        //                        replacements.Add("{n4_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(3).MaritalStatus));
        //                        replacements.Add("{n4_disability}", nomineeData.ElementAtOrDefault(3).Disability);

        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n4_name}", string.Empty);
        //                        replacements.Add("{n4_dob}", string.Empty);
        //                        replacements.Add("{n4_relation}", string.Empty);
        //                        replacements.Add("{n4_rate}", string.Empty);
        //                        replacements.Add("{n4_mStatus}", string.Empty);
        //                        replacements.Add("{n4_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 4)
        //                    {
        //                        replacements.Add("{n5_name}", nomineeData.ElementAtOrDefault(4).Name);
        //                        replacements.Add("{n5_dob}", DateBn(nomineeData.ElementAtOrDefault(4).DateOfBirth));
        //                        replacements.Add("{n5_relation}", nomineeData.ElementAtOrDefault(4).Relation);
        //                        replacements.Add("{n5_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(4).NominalRate));
        //                        replacements.Add("{n5_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(4).MaritalStatus));
        //                        replacements.Add("{n5_disability}", nomineeData.ElementAtOrDefault(4).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n5_name}", string.Empty);
        //                        replacements.Add("{n5_dob}", string.Empty);
        //                        replacements.Add("{n5_relation}", string.Empty);
        //                        replacements.Add("{n5_mStatus}", string.Empty);
        //                        replacements.Add("{n5_rate}", string.Empty);
        //                        replacements.Add("{n5_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 5)
        //                    {
        //                        replacements.Add("{n6_name}", nomineeData.ElementAtOrDefault(5).Name);
        //                        replacements.Add("{n6_dob}", DateBn(nomineeData.ElementAtOrDefault(5).DateOfBirth));
        //                        replacements.Add("{n6_relation}", nomineeData.ElementAtOrDefault(5).Relation);
        //                        replacements.Add("{n6_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(5).NominalRate));
        //                        replacements.Add("{n6_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(5).MaritalStatus));
        //                        replacements.Add("{n6_disability}", nomineeData.ElementAtOrDefault(5).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n6_name}", string.Empty);
        //                        replacements.Add("{n6_dob}", string.Empty);
        //                        replacements.Add("{n6_relation}", string.Empty);
        //                        replacements.Add("{n6_mStatus}", string.Empty);
        //                        replacements.Add("{n6_rate}", string.Empty);
        //                        replacements.Add("{n6_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 6)
        //                    {
        //                        replacements.Add("{n7_name}", nomineeData.ElementAtOrDefault(6).Name);
        //                        replacements.Add("{n7_dob}", DateBn(nomineeData.ElementAtOrDefault(6).DateOfBirth));
        //                        replacements.Add("{n7_relation}", nomineeData.ElementAtOrDefault(6).Relation);
        //                        replacements.Add("{n7_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(6).NominalRate));
        //                        replacements.Add("{n7_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(6).MaritalStatus));
        //                        replacements.Add("{n7_disability}", nomineeData.ElementAtOrDefault(6).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n7_name}", string.Empty);
        //                        replacements.Add("{n7_dob}", string.Empty);
        //                        replacements.Add("{n7_relation}", string.Empty);
        //                        replacements.Add("{n7_mStatus}", string.Empty);
        //                        replacements.Add("{n7_rate}", string.Empty);
        //                        replacements.Add("{n7_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 7)
        //                    {
        //                        replacements.Add("{n8_name}", nomineeData.ElementAtOrDefault(7).Name);
        //                        replacements.Add("{n8_dob}", DateBn(nomineeData.ElementAtOrDefault(7).DateOfBirth));
        //                        replacements.Add("{n8_relation}", nomineeData.ElementAtOrDefault(7).Relation);
        //                        replacements.Add("{n8_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(7).NominalRate));
        //                        replacements.Add("{n8_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(7).MaritalStatus));
        //                        replacements.Add("{n8_disability}", nomineeData.ElementAtOrDefault(7).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n8_name}", string.Empty);
        //                        replacements.Add("{n8_dob}", string.Empty);
        //                        replacements.Add("{n8_relation}", string.Empty);
        //                        replacements.Add("{n8_mStatus}", string.Empty);
        //                        replacements.Add("{n8_rate}", string.Empty);
        //                        replacements.Add("{n8_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 8)
        //                    {
        //                        replacements.Add("{n9_name}", nomineeData.ElementAtOrDefault(8).Name);
        //                        replacements.Add("{n9_dob}", DateBn(nomineeData.ElementAtOrDefault(8).DateOfBirth));
        //                        replacements.Add("{n9_relation}", nomineeData.ElementAtOrDefault(8).Relation);
        //                        replacements.Add("{n9_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(8).NominalRate));
        //                        replacements.Add("{n9_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(8).MaritalStatus));
        //                        replacements.Add("{n9_disability}", nomineeData.ElementAtOrDefault(8).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n9_name}", string.Empty);
        //                        replacements.Add("{n9_dob}", string.Empty);
        //                        replacements.Add("{n9_relation}", string.Empty);
        //                        replacements.Add("{n9_mStatus}", string.Empty);
        //                        replacements.Add("{n9_rate}", string.Empty);
        //                        replacements.Add("{n9_disability}", string.Empty);
        //                    }

        //                    if (nomineeData?.Count > 9)
        //                    {
        //                        replacements.Add("{n10_name}", nomineeData.ElementAtOrDefault(9).Name);
        //                        replacements.Add("{n10_dob}", DateBn(nomineeData.ElementAtOrDefault(9).DateOfBirth));
        //                        replacements.Add("{n10_relation}", nomineeData.ElementAtOrDefault(9).Relation);
        //                        replacements.Add("{n10_rate}", bn2enNumber(nomineeData.ElementAtOrDefault(9).NominalRate));
        //                        replacements.Add("{n10_mStatus}", TranlateMaritalStatusEnToBl(nomineeData.ElementAtOrDefault(9).MaritalStatus));
        //                        replacements.Add("{n10_disability}", nomineeData.ElementAtOrDefault(9).Disability);
        //                    }
        //                    else
        //                    {
        //                        replacements.Add("{n10_name}", string.Empty);
        //                        replacements.Add("{n10_dob}", string.Empty);
        //                        replacements.Add("{n10_relation}", string.Empty);
        //                        replacements.Add("{n10_mStatus}", string.Empty);
        //                        replacements.Add("{n10_rate}", string.Empty);
        //                        replacements.Add("{n10_disability}", string.Empty);
        //                    }

        //                    ReplacePlaceholders(doc, replacements);
        //                }

        //                if (PensionFormNames.Specimen_Signature == fileName || PensionFormNames.NoClaim_Certificate == fileName)
        //                {
        //                    // Replace placeholders
        //                    var replacements = new Dictionary<string, string>
        //                    {
        //                        {"name", data.Employee?.Name},
        //                        {"designation", data.Employee?.CurrentDesignation},
        //                        {"guardian",  data.Employee?.FatherName },
        //                    };
        //                    ReplacePlaceholders(doc, replacements);
        //                }

        //                if (PensionFormNames.ELPC == fileName)
        //                {
        //                    var replacements = new Dictionary<string, string>
        //                    {
        //                        {"name", data.Employee?.Name != null ? data.Employee?.Name: String.Empty},
        //                        {"NID", data.Employee?.NID != null ? data.Employee?.NID : String.Empty},
        //                        {"office", data.Employee?.Office != null ? data.Employee?.Office : String.Empty},
        //                        {"lastPost", data.Employee?.CurrentDesignation != null? data.Employee?.CurrentDesignation : String.Empty},
        //                        {"dob", DateBn(data.Employee?.DateOfBirth)},
        //                        { "JoiningDate",  DateBn(data.Employment?.JoiningDate ) },
        //                        { "DateOfRetirement", DateBn(data.Employment?.DateOfRetirement )},
        //                        { "StartDateOfPRL", DateBn(data.Employment ?.StartDateOfPRL)},
        //                        { "FinalRetirementDate", DateBn(data.Employment?.FinalRetirementDate)},
        //                        { "LastDrawnPayScale",  data.Employment?.LastDrawnPayScale != null ? data.Employment?.LastDrawnPayScale.ToString():String.Empty},
        //                        { "NextIncrementDate", DateBn(data.Employment ?.NextIncrementDate)},
        //                        { "AccountNo", data.ProvidentFundAccount?.AccountNo != null ? data.ProvidentFundAccount?.AccountNo.ToString(): String.Empty},
        //                        { "BookNo", data.ProvidentFundAccount?.BookNo != null ? data.ProvidentFundAccount?.BookNo.ToString(): String.Empty},
        //                        { "PageNo",  data.ProvidentFundAccount?.PageNo!= null ? data.ProvidentFundAccount?.PageNo.ToString(): String.Empty},
        //                        { "MonthlyDepositRate", data.ProvidentFundAccount?.MonthlyDepositRate != null ?data.ProvidentFundAccount?.MonthlyDepositRate.ToString(): String.Empty},
        //                        { "DepositMoneyBeforeJuneThirty", data.ProvidentFundAccount?.DepositMoneyBeforeJuneThirty != null ?data.ProvidentFundAccount?.DepositMoneyBeforeJuneThirty.ToString(): String.Empty},
        //                        { "PostRetirementTotalDeposit", data.ProvidentFundAccount?.PostRetirementTotalDeposit != null ? data.ProvidentFundAccount?.PostRetirementTotalDeposit.ToString():String.Empty},
        //                        { "TotalDeposit", data.ProvidentFundAccount?.TotalDeposit != null ? data.ProvidentFundAccount?.TotalDeposit.ToString(): String.Empty},
        //                        { "totalEarnedFull", data.EarnedLeave.First()?.LeaveEarned != null ?data.EarnedLeave.First()?.LeaveEarned.ToString():String.Empty},
        //                        { "enjoyedEarnedFull",data.EarnedLeave.First()?.LeaveConsumed!= null ? data.EarnedLeave.First()?.LeaveConsumed.ToString():String.Empty},
        //                        { "remainEarnedFull", data.EarnedLeave.First()?.LeaveRemaining!= null ?data.EarnedLeave.First()?.LeaveRemaining.ToString():String.Empty},
        //                        { "totalEarnedHalf", data.EarnedLeave.Last()?.LeaveEarned != null ? data.EarnedLeave.Last()?.LeaveEarned.ToString():String.Empty},
        //                        { "enjoyedEarnedHalf", data.EarnedLeave.Last()?.LeaveConsumed != null ? data.EarnedLeave.Last()?.LeaveConsumed.ToString():String.Empty},
        //                        { "remainEarnedHalf", data.EarnedLeave.Last()?.LeaveRemaining!= null ? data.EarnedLeave.Last()?.LeaveRemaining.ToString(): String.Empty},

        //                    };
        //                    ReplacePlaceholders(doc, replacements);
        //                }

        //                doc.Save();
        //            }

        //            tempFileStream.Position = 0;
        //        }

        //        var fileStreamResult = new FileStreamResult(tempFileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        //        {
        //            FileDownloadName = fileName
        //        };

        //        fileStreamResult.FileStream.Seek(0, SeekOrigin.Begin);

        //        // Register an OnCompleted event to delete the temporary file and dispose of the stream
        //        HttpContext.Response.RegisterForDispose(tempFileStream);
        //        HttpContext.Response.OnCompleted(() =>
        //        {
        //            System.IO.File.Delete(tempFilePath);
        //            return Task.CompletedTask;
        //        });

        //        return fileStreamResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Clean up if an exception occurred
        //        tempFileStream?.Dispose();
        //        System.IO.File.Delete(tempFilePath);

        //        return BadRequest("An error occurred while generating the document. Please try again later.");
        //    }
        //}

        ////public IActionResult DownloadDocumentWithData()
        ////{
        ////    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hello.docx"); 
        ////    string tempFilePath = Path.GetTempFileName(); // Generates a unique temporary file
        ////    FileStreamResult fileStreamResult = null;

        ////    try
        ////    {
        ////        using (FileStream templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        ////        using (FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
        ////        {
        ////            // Copy template to temporary file
        ////            templateStream.CopyTo(tempFileStream);
        ////            tempFileStream.Position = 0;

        ////            using (WordprocessingDocument doc = WordprocessingDocument.Open(tempFileStream, true))
        ////            {
        ////                // Replace placeholders
        ////                ReplacePlaceholder(doc, "name}", "John Doe");
        ////                ReplacePlaceholder(doc, "date}", DateTime.Now.ToShortDateString());

        ////                doc.Save();
        ////            }

        ////            tempFileStream.Position = 0;
        ////            fileStreamResult = File(tempFileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "GeneratedDocument.docx");              
        ////        }

        ////        // Delete the temporary file after streaming its content
        ////        System.IO.File.Delete(tempFilePath);

        ////        return fileStreamResult;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return BadRequest("An error occurred while generating the document. Please try again later.");
        ////    }
        ////}


        //[HttpGet, DisableRequestSizeLimit]
        //[ActionName("Download")]
        //public async Task<IActionResult> Download([FromQuery] string fileUrl, string fileName)
        //{
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl);

        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound();

        //    var memory = new MemoryStream();
        //    await using (var stream = new FileStream(filePath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, GetContentType(filePath), fileName);
        //}


        //#region private methods
        ////private void ReplacePlaceholder(WordprocessingDocument doc, string placeholder, string newValue)
        ////{
        ////    var body = doc.MainDocumentPart.Document.Body;
        ////    foreach (var text in body.Descendants<Text>())
        ////    {
        ////        if (text.Text.Contains(placeholder))
        ////        {
        ////            text.Text = text.Text.Replace(placeholder, newValue);
        ////        }
        ////    }
        ////}

        //private static void ReplacePlaceholders(WordprocessingDocument doc, Dictionary<string, string> replacements)
        //{
        //    var body = doc.MainDocumentPart.Document.Body;

        //    // Collect runs and their texts
        //    var runsAndTexts = body.Descendants<Run>()
        //                           .Select(r => new { Run = r, Texts = r.Descendants<Text>().ToList() })
        //                           .ToList();

        //    foreach (var runAndText in runsAndTexts)
        //    {
        //        string fullRunText = string.Concat(runAndText.Texts.Select(t => t.Text));
        //        bool replacementDone = false;

        //        foreach (var placeholder in replacements.Keys)
        //        {
        //            if (fullRunText.Contains(placeholder))
        //            {
        //                fullRunText = fullRunText.Replace(placeholder, replacements[placeholder]);
        //                replacementDone = true;
        //            }
        //        }

        //        if (replacementDone)
        //        {
        //            // Clear the existing texts and set the new one
        //            runAndText.Texts.ForEach(t => t.Remove());
        //            runAndText.Run.AppendChild(new Text(fullRunText));
        //        }
        //    }
        //}

        //private string GetContentType(string path)
        //{
        //    var provider = new FileExtensionContentTypeProvider();
        //    string contentType;

        //    if (!provider.TryGetContentType(path, out contentType))
        //    {
        //        contentType = "application/octet-stream";
        //    }

        //    return contentType;
        //}

        //private string DateBn(DateTime? SourceValue)

        //{
        //    if (SourceValue == null) return string.Empty;
        //    string Locale = "bn-BD";
        //    string format = "dd-MMMM-yyyy";
        //    var cultureInfo = CultureInfo.CreateSpecificCulture(Locale);
        //    cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();

        //    string formattedDate = SourceValue?.ToString(format, cultureInfo);
        //    string[] numerals = cultureInfo.NumberFormat.NativeDigits;

        //    for (int n = 0; n < numerals.Length; n++)
        //    {
        //        formattedDate = formattedDate.Replace(n.ToString(), numerals[n]);
        //    }
        //    return formattedDate;
        //}

        //private string bn2enNumber(double number)
        //{
        //    var en_number = number.ToString().Replace("1", "১").Replace("2", "২").Replace("3", "৩").Replace("4", "৪").Replace("5", "৫").Replace("6", "৬").Replace("7", "৭").Replace("8", "৮").Replace("9", "৯").Replace("0", "০");

        //    return en_number;
        //}


        //private string TranlateMaritalStatusEnToBl(string status)
        //{
        //    var statusMap = new Dictionary<string, string>
        //    {
        //        { "single", "অবিবাহিত" },
        //        { "married", "বিবাহিত" },
        //        { "widowed", "বিধবা" },
        //        { "divorced", "তালাকপ্রাপ্ত" }
        //    };

        //    if (statusMap.TryGetValue(status, out var translatedStatus))
        //    {
        //        return translatedStatus;
        //    }

        //    return String.Empty;
        //}

        //#endregion


    }
}
