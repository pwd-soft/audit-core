using PWD.Audit.Interfaces;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PWD.Audit.Services
{
    public class OfficeUserAppService : ApplicationService, IOfficeUserAppService
    {
        private readonly IRepository<OfficeUser, int> _repository;

        public OfficeUserAppService(IRepository<OfficeUser, int> repository)
        {
            _repository = repository;
        }

        public async Task<OfficeUserDto> CreateAsync(OfficeUserDto input)
        {
            var OfficeUser = ObjectMapper.Map<OfficeUserDto, OfficeUser>(input);
            var newOfficeUser = await _repository.InsertAsync(OfficeUser);

            return ObjectMapper.Map<OfficeUser, OfficeUserDto>(newOfficeUser);
        }

        public async Task<OfficeUserDto> UpdateAsync(OfficeUserDto input)
        {
            var dbItem = await _repository.FirstOrDefaultAsync(x=>x.UserId==input.UserId);

            if (dbItem is not null)
            {
                dbItem.IsActive = input.IsActive;
                var updatedItem = await _repository.UpdateAsync(dbItem);
                return ObjectMapper.Map<OfficeUser, OfficeUserDto>(updatedItem);
            }
            else
            {
                var item = ObjectMapper.Map<OfficeUserDto, OfficeUser>(input);
                await _repository.InsertAsync(item);
                return input;
            }



        }

        public async Task<OfficeUserDto> GetByIdAsync(int id)
        {
            var OfficeUser = await _repository.GetAsync(id);
            var OfficeUserDto = ObjectMapper.Map<OfficeUser, OfficeUserDto>(OfficeUser);
            return OfficeUserDto;
        }

        public async Task<List<OfficeUserDto>> GetListAsync() => ObjectMapper.Map<List<OfficeUser>, List<OfficeUserDto>>(await _repository.GetListAsync());

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<List<OfficeUserDto>> FetchOfficeUsers(OfficeUserDto dto)
        {
            var OfficeUsers = await _repository.GetListAsync(x => x.OfficeId == dto.OfficeId);
            var OfficeUserDto = ObjectMapper.Map<List<OfficeUser>, List<OfficeUserDto>>(OfficeUsers);
            return OfficeUserDto;
        }


        public async Task<bool> IsValid(OfficeUserDto dto) => await _repository.AnyAsync(x => x.UserId == dto.UserId && x.IsActive);
         
        //private async Task<List<OfficeUserDto>> AllData () => ObjectMapper.Map<List<OfficeUser>, List<OfficeUserDto>>(await _repository.GetListAsync());
    }
}
