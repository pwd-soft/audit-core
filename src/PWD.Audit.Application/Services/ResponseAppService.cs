using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Interfaces;
using PWD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace PWD.Audit.Services
{
    public class ResponseAppService : ApplicationService, IResponseAppService
    {
        private readonly IRepository<ResponseHistory, int> repository;

        public ResponseAppService(IRepository<ResponseHistory, int> _repository)
        {
            repository = _repository;
        }

        public async Task<ResponseHistoryDto> CreateAsync(ResponseHistoryDto input)
        {
            var responseHistory = ObjectMapper.Map<ResponseHistoryDto, ResponseHistory>(input);
            var newresponseHistory = await repository.InsertAsync(responseHistory, true);

            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(newresponseHistory);
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseHistoryDto> GetByIdAsync(int id)
        {
            //var responseWithDetails = await _repository.WithDetailsAsync(o => o.Associates, r => r.ResponseHistory);
            var responseWithDetails = await repository.GetAsync(r => r.Id == id);
            return ObjectMapper.Map<ResponseHistory, ResponseHistoryDto>(responseWithDetails);
        }

        public Task<List<ResponseHistoryDto>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ResponseHistoryDto>> GetListByObjectionIdAsync(int objectionId)
        {
            var responseWithDetails = await repository.GetListAsync(r => r.ObjectionId == objectionId);
            return ObjectMapper.Map<List<ResponseHistory>, List<ResponseHistoryDto>>(responseWithDetails);
        }

        public Task<ResponseHistoryDto> UpdateAsync(ResponseHistoryDto input)
        {
            throw new NotImplementedException();
        }
    }
}
