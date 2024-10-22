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
            var newObjection = await _repository.InsertAsync(objection,true);

            return ObjectMapper.Map<Objection, ObjectionDto>(newObjection);
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

            var newAssociates = objectionInput.Associates.Where(a => a.Id < 0).ToList();
            if (newAssociates.Any())
            {
                foreach (var item in newAssociates)
                {
                    item.Id = 0;
                    item.ObjectionId = updatedItem.Id;
                }

                var newAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(newAssociates);
                if (newAssociatesEntity.Count() > 1)
                {
                    await _associateRepository.InsertManyAsync(newAssociatesEntity);
                }
                else
                {
                    await _associateRepository.InsertAsync(newAssociatesEntity.FirstOrDefault());
                }
            }

            var updateAssociates = objectionInput.Associates.Where(a => a.Id > 0).ToList();
            if (updateAssociates.Any())
            {
                var updateAssociatesEntity = ObjectMapper.Map<List<AssociateDto>, List<Associate>>(updateAssociates);
                if (updateAssociatesEntity.Count() > 1)
                {
                    await _associateRepository.UpdateManyAsync(updateAssociatesEntity);
                }
                else
                {
                    await _associateRepository.UpdateAsync(updateAssociatesEntity.FirstOrDefault());
                }
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
            
            //var objectionList = queryableList.Skip(filterCriteria.Offset)
            //    .Take(filterCriteria.Limit).ToList();

            return ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objectionList);
        }

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        //private async Task<List<ObjectionDto>> AllData () => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());
    }
}
