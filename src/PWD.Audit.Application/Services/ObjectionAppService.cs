using PWD.Audit.Enum;
using PWD.Audit.Interfaces;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.Models;
using PWD.Audit.Helper;

namespace PWD.Audit.Services
{
    public class ObjectionAppService : ApplicationService, IObjectionAppService
    {
        private readonly IRepository<Objection, int> _repository;
        private readonly IRepository<Associate, int> _associateRepository;
        private readonly IRepository<ResponseHistory, int> _responseHistoryRepository;
        private readonly IRepository<ResponseState, int> _responseStateRepository;
        private readonly IRepository<ResponseComment, int> _responseCommentRepository;
        private readonly IAttachmentAppService _attachmentService;

        public ObjectionAppService(IRepository<Objection, int> repository, IRepository<Associate, int> associateRepository, IRepository<ResponseState, int> responseStateRepository,
            IRepository<ResponseHistory, int> responseHistoryRepository, IRepository<ResponseComment, int> responseCommentRepository, IAttachmentAppService attachmentService)
        {
            _repository = repository;
            _associateRepository = associateRepository;
            _responseStateRepository = responseStateRepository;
            _responseHistoryRepository = responseHistoryRepository;
            _responseCommentRepository = responseCommentRepository;
            _attachmentService = attachmentService;
        }

        public async Task<ObjectionDto> CreateAsync(ObjectionDto objectionInput)
        {
            var objection = ObjectMapper.Map<ObjectionDto, Objection>(objectionInput);
            var newObjection = await _repository.InsertAsync(objection, true);

            if (objectionInput.Attachments?.Count > 0)
            {
                foreach (var attachment in objectionInput.Attachments)
                {
                    attachment.ObjectionId = newObjection.Id;
                    attachment.AttachmentType = AttachmentType.Objection;
                }
                // Process attachments to upload folder
                FileProcessing.PorcessFilesToUploadFolder(newObjection.Id, objectionInput.Attachments);
                _attachmentService.InsertBulkAsync(objectionInput.Attachments).GetAwaiter().GetResult();
            }

            //var updateAttachmentField = await _repository.GetAsync(newObjection.Id);
            //updateAttachmentField.Attachments = JsonSerializer.Serialize(objectionInput.FileDataInput);
            //await _repository.UpdateAsync(updateAttachmentField);

            var responseState = new ResponseState
            {
                ObjectionId = newObjection.Id,
                Office = objectionInput.OfficeCode,
                User = objectionInput.OfficeCode,
                Note = $"Initiated from {objectionInput.OfficeCode}",
                IsLocked = false
            };
            await _responseStateRepository.InsertAsync(responseState, true);

            return ObjectMapper.Map<Objection, ObjectionDto>(newObjection);
        }

        //private List<FileDataInput> ProcessAttachments(int objectionId, List<FileDataInput> fileDataInput, string attachments)
        //{
        //    var existingAttachments = new List<FileDataInput>();

        //    //Processing proper image path
        //    fileDataInput = PorcessFilesToUploadFolder(objectionId, fileDataInput);

        //    if (attachments is not null)
        //    {
        //        existingAttachments = JsonSerializer.Deserialize<FileDataInput[]>(attachments).ToList();
        //    }

        //    //Assigning id to every attachment by generating random numbers between 1100-2000 and checking,
        //    //if the id exists generate new id; then assign the id
        //    foreach (var file in fileDataInput)
        //    {
        //        Random rnd = new Random();
        //        int newId = rnd.Next(1100, 2000);
        //        while (existingAttachments.Exists(f => f.Id == newId))
        //        {
        //            newId = rnd.Next(1100, 2000);
        //        }
        //        file.Id = newId;
        //        existingAttachments.Add(file);
        //    }

        //    return existingAttachments;
        //}

        //private List<AttachmentDto> PorcessFilesToUploadFolder(int objectionId, List<AttachmentDto> fileDataInput)

        //private void PorcessFilesToUploadFolder(int objectionId, List<AttachmentDto> fileDataInput)
        //{
        //    var directoryName = objectionId.ToString();
        //    var folderName = Path.Combine("wwwroot", "Uploaded_Documents", directoryName);
        //    if (!Directory.Exists(folderName))
        //    {
        //        DirectoryInfo di = Directory.CreateDirectory(folderName);
        //    }

        //    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        //    foreach (var file in fileDataInput)
        //    {
        //        var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.Path);
        //        var destinationPath = Path.Combine(pathToSave, file.FileName);

        //        System.IO.File.Copy(sourcePath, destinationPath, true);
        //        System.IO.File.Delete(sourcePath);
        //        var savedFileName = file.Path.Split(@"\")[1];
        //        var path = Path.Combine(folderName, savedFileName);
        //        path = path.Replace(@"wwwroot\", string.Empty);

        //        file.Path = path;
        //    }

        //    //return fileDataInput;
        //}

        public async Task<ObjectionDto> UpdateAsync(ObjectionDto objectionInput)
        {
            var dbItem = await _repository.GetAsync(objectionInput.Id);

            //var newFiles = objectionInput.

            if (dbItem is not null)
            {
                //dbItem.Date = objectionInput.Date;
                dbItem.FinancialYear = objectionInput.FinancialYear;
                dbItem.ObjectionType = objectionInput.ObjectionType;
                dbItem.DirectorateType = objectionInput.DirectorateType;
                dbItem.Name = objectionInput.Name;
                dbItem.Description = objectionInput.Description;
                dbItem.Response = objectionInput.Response;
                dbItem.Value = objectionInput.Value;
                dbItem.ObjectionStatus = objectionInput.ObjectionStatus;
                dbItem.IsActive = objectionInput.IsActive;
                dbItem.Note = objectionInput.Note;
                dbItem.MemoNumber = objectionInput.MemoNumber;
                dbItem.MemoDate = objectionInput.MemoDate;
                dbItem.ArticleNumber = objectionInput.ArticleNumber;
                dbItem.ObjectionMemoNumber = objectionInput.ObjectionMemoNumber;
                dbItem.ObjectionDate = objectionInput.ObjectionDate;

                //if (objectionInput.FileDataInput?.Count > 0)
                //{
                //    objectionInput.FileDataInput = ProcessAttachments(dbItem.Id, objectionInput.FileDataInput, dbItem.Attachments);
                //    dbItem.Attachments = JsonSerializer.Serialize(objectionInput.FileDataInput);
                //}
            }

            var updatedItem = await _repository.UpdateAsync(dbItem);
            if (objectionInput?.Associates?.Count > 0)
            {
                var newAssociates = objectionInput.Associates.Where(a => a.Id == 0).ToList();
                if (newAssociates.Any())
                {
                    newAssociates.ForEach(x => x.ObjectionId = updatedItem.Id);
                    var newAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(newAssociates);
                    await _associateRepository.InsertManyAsync(newAssociatesEntity);
                }

                var updateAssociates = objectionInput.Associates.Where(a => a.Id > 0).ToList();
                if (updateAssociates.Any())
                {
                    var updateAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(updateAssociates);
                    await _associateRepository.UpdateManyAsync(updateAssociatesEntity);
                }
            }

            var newAttachments = objectionInput.Attachments.Where(a => a.ObjectionId == 0).ToList();
            if (newAttachments.Count > 0)
            {
                foreach (var attachment in objectionInput.Attachments)
                {
                    attachment.ObjectionId = objectionInput.Id;
                    attachment.AttachmentType = AttachmentType.Objection;
                }
                // Process attachments to upload folder
                FileProcessing.PorcessFilesToUploadFolder(objectionInput.Id, objectionInput.Attachments);
                _attachmentService.InsertBulkAsync(objectionInput.Attachments).GetAwaiter().GetResult();
            }

            return ObjectMapper.Map<Objection, ObjectionDto>(updatedItem);
        }

        public async Task<ObjectionDto> GetDetailsByIdAsync(int id)
        {
            var objectionWithDetails = await _repository.WithDetailsAsync(o => o.Associates, r => r.ResponseHistories, a => a.Attachments);
            var objection = objectionWithDetails.FirstOrDefault(o => o.Id == id);
            foreach (var responseHistory in objection.ResponseHistories)
            {
                responseHistory.ResponseStates = await _responseStateRepository.GetListAsync(x => x.ResponseHistoryId == responseHistory.Id);
                //if (responseHistory.ResponseStates.Count == 0)
                //{
                //    responseHistory.ResponseStates = await _responseStateRepository.GetListAsync(x => x.ObjectionId == objection.Id);
                //}
                responseHistory.ResponseComments = await _responseCommentRepository.GetListAsync(x => x.ResponseHistoryId == responseHistory.Id);
            }
            ;

            var objectionDto = ObjectMapper.Map<Objection, ObjectionDto>(objection);

            foreach (var responseHistory in objectionDto.ResponseHistories)
            {
                responseHistory.Attachments = await _attachmentService.GetListResponseIdAsync(responseHistory.Id, AttachmentType.Response);
            }

            return objectionDto;
        }

        public async Task<List<ObjectionDto>> GetListAsync() => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());

        public async Task<List<ObjectionDto>> GetListByOfficeCodeAsync(string officeCode)
        {
            var objectionList = await _repository.GetListAsync(i => i.OfficeCode == officeCode);
            return ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);
        }

        public async Task<List<ObjectionDto>> SearchObjections(ObjectionFilterModel filterCriteria)
        {
            var queryableList = await _repository.GetQueryableAsync();
            //queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.OfficeCode is not null)
                queryableList = queryableList.Where(o => o.OfficeCode == filterCriteria.OfficeCode);

            if (filterCriteria.DirectorateType > 0)
                queryableList = queryableList.Where(o => o.DirectorateType == filterCriteria.DirectorateType);

            if (filterCriteria.ObjectionType > 0)
                queryableList = queryableList.Where(o => o.ObjectionType == filterCriteria.ObjectionType);

            if (!String.IsNullOrEmpty(filterCriteria.FinancialYear))
                queryableList = queryableList.Where(o => o.FinancialYear == filterCriteria.FinancialYear);

            //if (filterCriteria.IsBroadSheet)
            //    queryableList = queryableList.Where(o => o.IsBroadSheet == filterCriteria.IsBroadSheet);

            //if (filterCriteria.IsResolved)
            //    queryableList = queryableList.Where(o => o.IsResolved == filterCriteria.IsResolved);

            var objectionList = queryableList.ToList();

            objectionList = queryableList
                .Skip(filterCriteria.Offset)
                .Take(filterCriteria.Limit)
                .ToList();

            return ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);
        }

        public async Task<GenericListDto<ObjectionDto>> SearchObjectionsWithPaging(ObjectionFilterModel filterCriteria)
        {
            GenericListDto<ObjectionDto> objectionList = new GenericListDto<ObjectionDto>();

            var queryableList = await _repository.GetQueryableAsync();
            //queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.OfficeCode is not null)
                queryableList = queryableList.Where(o => o.OfficeCode == filterCriteria.OfficeCode);

            if (filterCriteria.DirectorateType > 0)
                queryableList = queryableList.Where(o => o.DirectorateType == filterCriteria.DirectorateType);

            if (filterCriteria.ObjectionType > 0)
                queryableList = queryableList.Where(o => o.ObjectionType == filterCriteria.ObjectionType);

            if (!String.IsNullOrEmpty(filterCriteria.FinancialYear))
                queryableList = queryableList.Where(o => o.FinancialYear == filterCriteria.FinancialYear);

            if (filterCriteria.ObjectionStatus > 0)
                queryableList = queryableList.Where(o => o.ObjectionStatus == filterCriteria.ObjectionStatus);

            //if (filterCriteria.IsResolved)
            //    queryableList = queryableList.Where(o => o.IsResolved == filterCriteria.IsResolved);


            objectionList.CountData = queryableList.Count();

            queryableList = queryableList
                //.OrderByDescending(o => o.CreationTime)
                .Skip(filterCriteria.Offset)
                .Take(filterCriteria.Limit);

            objectionList.ListData = ObjectMapper.Map<IQueryable<Objection>, List<ObjectionDto>>(queryableList);
            var oIds = objectionList.ListData.Select(o => o.Id).ToList();
            var data = await _attachmentService.GetObjectionAttachmentCount(oIds, AttachmentType.Objection);

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    if (item.ObjectionCount > 0)
                    {
                        var objectionDto = objectionList.ListData.FirstOrDefault(o => o.Id == item.ObjectionId);
                        if (objectionDto != null)
                        {
                            objectionDto.HasAttachment = true;
                        }
                    }
                }
            }

            return objectionList;
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        //private async Task<List<ObjectionDto>> AllData () => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());

        public async Task<List<ObjectionDto>> GetIncomingResponseListAsync(string officeCode)
        {
            const string AuditEE = "ee_audit";
            var states = await _responseStateRepository.WithDetailsAsync();
            var stateList = states.Where(i => i.User == officeCode && i.IsLocked == false).ToList();
            if (officeCode == "se_audit")
                stateList.AddRange(states.Where(i => i.User == AuditEE && i.IsLocked == false).ToList());
            var objectionIds = stateList.Select(s => s.ObjectionId).Distinct().ToList();
            var objections = await _repository.WithDetailsAsync(r => r.ResponseHistories);
            var objectionList = objections.Where(i => objectionIds.Contains(i.Id)).ToList();
            var objectionListDto = ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);

            foreach (var objectionDto in objectionListDto)
            {
                if (objectionDto.ResponseHistories.Count > 0)
                {
                    var latestResponse = objectionDto.ResponseHistories.OrderByDescending(r => r.Id).FirstOrDefault();
                    if (latestResponse != null)
                    {
                        objectionDto.ResponseHistories = new List<ResponseHistoryDto> { latestResponse };
                    }
                }
            }

            return objectionListDto;
        }

        public async Task<ObjectionDto> GetByIdAsync(int id)
        {
            var objection = await _repository.GetAsync(id);
            if (objection == null)
            {
                throw new KeyNotFoundException($"Objection with ID {id} not found.");
            }
            return ObjectMapper.Map<Objection, ObjectionDto>(objection);
        }

    }
}
