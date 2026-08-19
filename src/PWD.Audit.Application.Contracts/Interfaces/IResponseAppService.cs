using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IResponseAppService
    {
        Task<ResponseDto> CreateAsync(ResponseDto input);
        Task DeleteAsync(int id);
        Task<ResponseDto> GetByIdAsync(int id);
        Task<List<ResponseDto>> GetListAsync();
        Task<ResponseDto> UpdateAsync(ResponseDto input);
        Task<List<ResponseDto>> GetListByObjectionIdAsync(int objectionId);
        Task<ResponseStateDto> UpdateResponseStateAsync(ResponseStateDto responseStateDto);
    }
}
