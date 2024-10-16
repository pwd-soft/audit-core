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
            var newObjection = await _repository.InsertAsync(objection);

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
            return ObjectMapper.Map<Objection, ObjectionDto>(updatedItem);
        }

        public async Task<ObjectionDto> GetByIdAsync(int id)
        {
            var objection = await _repository.GetAsync(id);
            var objectionDto = ObjectMapper.Map<Objection, ObjectionDto>(objection);
            return objectionDto;
        }

        public async Task<List<ObjectionDto>> GetListAsync() => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        //private async Task<List<ObjectionDto>> AllData () => ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(await _repository.GetListAsync());
    }
}
