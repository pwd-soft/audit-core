using Microsoft.AspNetCore.Authorization;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
using PWD.Audit.Helper;
using PWD.Audit.InputDtos;
using PWD.Audit.Interfaces;
using PWD.Audit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace PWD.Audit.Services
{
    [Authorize]
    public class ResponseAppService : ApplicationService, IResponseAppService
    {
        private readonly IRepository<ResponseHistory, int> _repository;
        private readonly IRepository<ResponseState, int> _responseStateRepository;
        private readonly IObjectionAppService _objectionService;
        private readonly IApprovalAppService _approvalService;
        private readonly IAttachmentAppService _attachmentService;

        public ResponseAppService(IRepository<ResponseHistory, int> _repository, IRepository<ResponseState, int> responseStateRepository, IObjectionAppService objectionService, IApprovalAppService approvalService, IAttachmentAppService attachmentService)
        {
            this._repository = _repository;
            _objectionService = objectionService;
            _responseStateRepository = responseStateRepository;
            _approvalService = approvalService;
            _attachmentService = attachmentService;
        }

        public async Task<ResponseHistoryDto> CreateAsync(ResponseHistoryDto input)
        {
            var responseHistory = ObjectMapper.Map<ResponseHistoryDto, ResponseHistory>(input);
            var newresponseHistory = await _repository.InsertAsync(responseHistory, true);

            //if (input.FileDataInput?.Count > 0)
            //{
            //    input.FileDataInput = ProcessAttachments(newresponseHistory.Id, input.FileDataInput, input.Attachments);
            //}

            //var updateAttachmentField = await _repository.GetAsync(newresponseHistory.Id);
            //updateAttachmentField.Attachments = JsonSerializer.Serialize(input.FileDataInput);
            //await _repository.UpdateAsync(updateAttachmentField);

            var userInfo = await _approvalService.GetPosting(input.User);

            var responseState = new ResponseState
            {
                ResponseHistoryId = newresponseHistory.Id,
                ObjectionId = input.ObjectionId,
                Office = input.User,
                User = input.User,
                PostingId = userInfo.PostingId,
                IsLocked = false,
                Note = $"Response initiated from {input.User}"
            };
            await _responseStateRepository.InsertAsync(responseState, true);

            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(newresponseHistory);
            //return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(new ResponseHistory());
        }

        private List<FileDataInput> ProcessAttachments(int objectionId, List<FileDataInput> fileDataInput, string attachments)
        {
            var existingAttachments = new List<FileDataInput>();

            //Processing proper image path
            fileDataInput = PorcessFilesToUploadFolder(objectionId, fileDataInput);

            if (attachments is not null)
            {
                existingAttachments = JsonSerializer.Deserialize<FileDataInput[]>(attachments).ToList();
            }

            //Assigning id to every attachment by generating random numbers between 1100-2000 and checking,
            //if the id exists generate new id; then assign the id
            foreach (var file in fileDataInput)
            {
                Random rnd = new Random();
                int newId = rnd.Next(1100, 2000);
                while (existingAttachments.Exists(f => f.Id == newId))
                {
                    newId = rnd.Next(1100, 2000);
                }
                file.Id = newId;
                existingAttachments.Add(file);
            }

            return existingAttachments;
        }

        private List<FileDataInput> PorcessFilesToUploadFolder(int objectionId, List<FileDataInput> fileDataInput)
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

            return fileDataInput;
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseHistoryDto> GetByIdAsync(int id)
        {
            //var responseWithDetails = await _repository.WithDetailsAsync(o => o.Associates, r => r.ResponseHistory);
            var responses = await _repository.WithDetailsAsync(r => r.ResponseComments);
            var responseWithDetails = responses.FirstOrDefault(r => r.Id == id);
            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(responseWithDetails);
        }

        public Task<List<ResponseHistoryDto>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ResponseHistoryDto>> GetListByObjectionIdAsync(int objectionId)
        {
            var responseWithDetails = await _repository.GetListAsync(r => r.ObjectionId == objectionId);
            return ObjectMapper.Map<List<ResponseHistory>, List<ResponseHistoryDto>>(responseWithDetails);
        }

        public async Task<ResponseHistoryDto> UpdateAsync(ResponseHistoryDto input)
        {
            var response = await _repository.GetAsync(r => r.Id == input.Id);

            response.Response = input.Response;
            //response.LockStatus = input.LockStatus;

            //if (input.FileDataInput?.Count > 0)
            //{
            //    input.FileDataInput = ProcessAttachments(response.Id, input.FileDataInput, response.Attachments);
            //    response.Attachments = JsonSerializer.Serialize(input.FileDataInput);
            //}

            var updatedResponse = await _repository.UpdateAsync(response);

            if(input.Attachments.Count > 0)
            {
                foreach (var attachment in input.Attachments)
                {
                    attachment.ObjectionId = input.ObjectionId;
                    attachment.ResponseId = response.Id;
                    attachment.AttachmentType = AttachmentType.Response;
                }
                // Process attachments to upload folder
                FileProcessing.PorcessFilesToUploadFolder(input.ObjectionId, input.Attachments.ToList());
                _attachmentService.InsertBulkAsync(input.Attachments).GetAwaiter().GetResult();
            }

            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(response);
        }

        //public async Task UpdateResponseFlow()
        //{
        //    var objectionList = await _objectionRepository.GetListAsync();
        //    foreach (var objection in objectionList)
        //    {
        //        //var response = new ResponseHistory
        //        //{
        //        //    ObjectionId = objection.Id,
        //        //    User = "ee_dhk1"
        //        //};
        //        //var newResponseHistory = await _responseHistoryRepository.InsertAsync(response, true);

        //        var responseState = new ResponseState
        //        {
        //            ObjectionId = objection.Id,
        //            Office = objection.OfficeCode,
        //            User = "ee_dhk1",
        //            //PostingId = 19362,
        //            IsLocked = false,
        //        };
        //        await _responseStateRepository.InsertAsync(responseState, true);

        //    }
        //}

        public async Task<ResponseStateDto> UpdateResponseStateAsync(ResponseStateDto responseStateDto)
        {
            var responseByObjection = await _responseStateRepository.GetListAsync(r => r.ObjectionId == responseStateDto.ObjectionId);
            var lastResponseState = responseByObjection.OrderByDescending(r => r.CreationTime).FirstOrDefault();
            if (lastResponseState != null)
            {
                //throw new Exception("Cannot update response state as it is locked.");
                if (lastResponseState.ResponseHistoryId == 0)
                {
                    lastResponseState.ResponseHistoryId = responseStateDto.ResponseHistoryId;
                }
                lastResponseState.IsLocked = true;
                await _responseStateRepository.UpdateAsync(lastResponseState);
            }

            var newResponseState = ObjectMapper.Map<ResponseStateDto, ResponseState>(responseStateDto);
            newResponseState = await _responseStateRepository.InsertAsync(newResponseState);

            return ObjectMapper.Map<ResponseState, ResponseStateDto>(newResponseState);
        }
        
        public async Task<ResponseHistoryDto> UpdateResponseStatus(ResponseHistoryDto input)
        {
            var responseStates = await _responseStateRepository.GetListAsync(r => r.ResponseHistoryId == input.Id);
            var latestState = responseStates.OrderByDescending(r => r.CreationTime).FirstOrDefault();

            var response = _repository.FirstOrDefault(r => r.Id == input.Id);
            if (response != null)
            {
                response.Status = input.Status;
                response.User = latestState.User;
                response = await _repository.UpdateAsync(response);
            }

            // Adding Response Attachments
            if (input.Attachments?.Count > 0)
            {
                foreach (var attachment in input.Attachments)
                {
                    attachment.ObjectionId = input.ObjectionId;
                    attachment.ResponseId = response.Id;
                    attachment.AttachmentType = AttachmentType.ResponseRejected;
                }
                // Process attachments to upload folder
                FileProcessing.PorcessFilesToUploadFolder(input.ObjectionId, input.Attachments.ToList());
                _attachmentService.InsertBulkAsync(input.Attachments).GetAwaiter().GetResult();
            }

            if (input.Status == ResponseStatus.RejectedByMinistry)
            {
                if (latestState != null)
                {
                    latestState.Note = input.Response;
                    latestState.IsLocked = true;
                    await _responseStateRepository.UpdateAsync(latestState);
                }
            }

            if (input.Status == ResponseStatus.Rejected  || input.Status == ResponseStatus.RejectedByMinistry)
            {
                var objection = await _objectionService.GetByIdAsync(input.ObjectionId);
                if (objection != null)
                {
                    if (objection.ObjectionType == ObjectionType.NonSFI)
                    {
                        objection.ObjectionStatus = ObjectionStatus.RequestedReAnswerFromMinistry;
                    }
                    if (objection.ObjectionType == ObjectionType.SFI)
                    {
                        objection.ObjectionStatus = ObjectionStatus.RequestedReAnswerFromAGOffice;
                    }

                    await _objectionService.UpdateAsync(objection);
                }
            }

            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(response);
        }
    }
}
