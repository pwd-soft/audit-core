using AutoMapper;
using Nito.AsyncEx;
using PWD.Audit.Enum;
using PWD.Audit.Interfaces;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.ObjectMapping;
using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.InputDtos;
using System.IO;
using System.Net.Mail;
using System.Text.Json;

namespace PWD.Audit.Services
{
    public class ObjectionAppService : ApplicationService, IObjectionAppService
    {
        private readonly IRepository<Objection, int> _repository;
        private readonly IRepository<Associate, int> _associateRepository;

        public ObjectionAppService(IRepository<Objection, int> repository, IRepository<Associate, int> associateRepository)
        {
            _repository = repository;
            _associateRepository = associateRepository;
        }

        public async Task<ObjectionDto> CreateAsync(ObjectionDto objectionInput)
        {
            var objection = ObjectMapper.Map<ObjectionDto, Objection>(objectionInput);
            var newObjection = await _repository.InsertAsync(objection, true);

            if (objectionInput.FileDataInput.Count > 0) 
            {
                //await FilePorcessing(newObjection.Id, objectionInput.FileDataInput);
                objectionInput.FileDataInput = PorcessFilesToUploadFolder(newObjection.Id, objectionInput.FileDataInput);
            }

            var updateAttachmentField = await _repository.GetAsync(newObjection.Id);
            updateAttachmentField.Attachments = JsonSerializer.Serialize(objectionInput.FileDataInput);
            await _repository.UpdateAsync(updateAttachmentField);

            return ObjectMapper.Map<Objection, ObjectionDto>(newObjection);
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

                var path = Path.Combine(folderName, file.Path);
                path = path.Replace(@"wwwroot\", string.Empty);

                file.Path = path;
            }

            return fileDataInput;
        }

        public async Task<ObjectionDto> UpdateAsync(ObjectionDto objectionInput)
        {
            var dbItem = await _repository.GetAsync(objectionInput.Id);

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
                dbItem.IsBroadSheet = objectionInput.IsBroadSheet;
                dbItem.IsResolved = objectionInput.IsResolved;
                dbItem.IsActive = objectionInput.IsActive;
                dbItem.Note = objectionInput.Note;
                //dbItem.Attachments = objectionInput.Attachments;
                //dbItem.< AssociateDto > Associates = objectionInput.< AssociateDto > Associates;
            }

            var updatedItem = await _repository.UpdateAsync(dbItem);

            var newAssociates = objectionInput.Associates.Where(a => a.Id == 0).ToList();
            if (newAssociates.Any())
            {
                newAssociates.ForEach(x=>x.ObjectionId=updatedItem.Id);
                var newAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(newAssociates);
                await _associateRepository.InsertManyAsync(newAssociatesEntity);
                //if (newAssociatesEntity.Count() > 1)
                //    await _associateRepository.InsertManyAsync(newAssociatesEntity);
                //else
                //    await _associateRepository.InsertAsync(newAssociatesEntity.FirstOrDefault());
            }

            var updateAssociates = objectionInput.Associates.Where(a => a.Id > 0).ToList();
            if (updateAssociates.Any())
            {
                var updateAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(updateAssociates);
                await _associateRepository.UpdateManyAsync(updateAssociatesEntity);
                //if (updateAssociatesEntity.Count() > 1)
                //    await _associateRepository.UpdateManyAsync(updateAssociatesEntity);
                //else
                //    await _associateRepository.UpdateAsync(updateAssociatesEntity.FirstOrDefault());
            }

            return ObjectMapper.Map<Objection, ObjectionDto>(updatedItem);
        }

        public async Task<ObjectionDto> GetByIdAsync(int id)
        {
            var objectionWithDetails = await _repository.WithDetailsAsync(o => o.Associates);
            var objection = objectionWithDetails.FirstOrDefault(o => o.Id == id);
            return ObjectMapper.Map<Objection, ObjectionDto>(objection);
        }

        public async Task<List<ObjectionDto>> GetListAsync() => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());

        //to be removed, work with FilterObjections
        public async Task<List<ObjectionDto>> GetListByOfficeIdAsync(Guid officeId)
        {
            var objectionList = await _repository.GetListAsync(i => i.OfficeId == officeId);
            return ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);
        }

        public async Task<List<ObjectionDto>> SearchObjections(ObjectionFilterModel filterCriteria)
        {
            var queryableList = await _repository.GetQueryableAsync();
            //queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.OfficeId is not null)
                queryableList = queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.DirectorateType > 0)
                queryableList = queryableList.Where(o => o.DirectorateType == filterCriteria.DirectorateType);

            if (filterCriteria.ObjectionType > 0)
                queryableList = queryableList.Where(o => o.ObjectionType == filterCriteria.ObjectionType);

            if (!String.IsNullOrEmpty(filterCriteria.FinancialYear))
                queryableList.Where(o => o.FinancialYear == filterCriteria.FinancialYear);

            if (filterCriteria.IsBroadSheet)
                queryableList = queryableList.Where(o => o.IsBroadSheet == filterCriteria.IsBroadSheet);

            if (filterCriteria.IsResolved)
                queryableList = queryableList.Where(o => o.IsResolved == filterCriteria.IsResolved);

            var objectionList = queryableList.ToList();

            objectionList = queryableList.Skip(filterCriteria.Offset)
                .Take(filterCriteria.Limit).ToList();

            return ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);
        }

        public async Task<GenericListDto<ObjectionDto>> SearchObjectionsWithPaging(ObjectionFilterModel filterCriteria)
        {
            GenericListDto<ObjectionDto> objectionList = new GenericListDto<ObjectionDto>();

            var queryableList = await _repository.GetQueryableAsync();
            //queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.OfficeId is not null)
                queryableList = queryableList.Where(o => o.OfficeId == filterCriteria.OfficeId);

            if (filterCriteria.DirectorateType > 0)
                queryableList = queryableList.Where(o => o.DirectorateType == filterCriteria.DirectorateType);

            if (filterCriteria.ObjectionType > 0)
                queryableList = queryableList.Where(o => o.ObjectionType == filterCriteria.ObjectionType);

            if (!String.IsNullOrEmpty(filterCriteria.FinancialYear))
                queryableList.Where(o => o.FinancialYear == filterCriteria.FinancialYear);

            if (filterCriteria.IsBroadSheet)
                queryableList = queryableList.Where(o => o.IsBroadSheet == filterCriteria.IsBroadSheet);

            if (filterCriteria.IsResolved)
                queryableList = queryableList.Where(o => o.IsResolved == filterCriteria.IsResolved);


            objectionList.CountData = queryableList.Count();

            queryableList = queryableList
                //.OrderByDescending(o => o.CreationTime)
                .Skip(filterCriteria.Offset)
                .Take(filterCriteria.Limit);

            objectionList.ListData = ObjectMapper.Map<IQueryable<Objection>, List<ObjectionDto>>(queryableList);

            return objectionList;
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        //private async Task<List<ObjectionDto>> AllData () => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());
    }
}
