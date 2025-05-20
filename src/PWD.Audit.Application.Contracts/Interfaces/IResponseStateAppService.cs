using PWD.Audit.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IResponseStateAppService
    {
        Task<ObjectionDto> CreateAsync(ObjectionDto input);
        Task<ObjectionDto> GetByIdAsync(int id);
        Task<List<ObjectionDto>> GetListAsync();
        Task<ObjectionDto> UpdateAsync(ObjectionDto input);
    }    
}
