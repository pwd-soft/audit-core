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

namespace PWD.Audit.Services
{
    public class ResponseCommentAppService : ApplicationService, IResponseCommentAppService
    {
        private readonly IRepository<ResponseComment, int> _repository;

        public ResponseCommentAppService(IRepository<ResponseComment, int> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseCommentDto> CreateAsync(ResponseCommentDto input)
        {
            var responseComment = ObjectMapper.Map<ResponseCommentDto, ResponseComment>(input);
            //await _repository.InsertAsync(responseComment, true);
            await _repository.InsertAsync(responseComment, true);
            return ObjectMapper.Map<ResponseComment, ResponseCommentDto>(responseComment);
        }

        public async Task<ResponseCommentDto> GetByIdAsync(int id)
        {
            var responseComment = await _repository.GetAsync(r => r.Id == id);
            return ObjectMapper.Map<ResponseComment, ResponseCommentDto>(responseComment);
        }

        public Task<List<ResponseCommentDto>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseCommentDto> UpdateAsync(ResponseCommentDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseCommentDto> GetSuperiorLevelComment(int responseHistoryId, string superiorOfficeCode)
        {
            var responseComments = await _repository.GetListAsync(r => r.ResponseHistoryId == responseHistoryId && r.User == superiorOfficeCode);
            var superiorCommentDto = new ResponseCommentDto();
            if (responseComments.Any())
            {
                if(responseComments.Count > 1) 
                {
                    var responseComment = responseComments.OrderByDescending(c => c.CreationTime).FirstOrDefault();
                    superiorCommentDto = ObjectMapper.Map<ResponseComment, ResponseCommentDto>(responseComment);
                }
                else
                {
                    return ObjectMapper.Map<ResponseComment, ResponseCommentDto>(responseComments.FirstOrDefault());
                }
            }
            
            return superiorCommentDto;
        }
    }
}
