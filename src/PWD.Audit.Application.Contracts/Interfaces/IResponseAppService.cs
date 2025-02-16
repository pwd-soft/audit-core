using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IResponseAppService
    {
        Task<ResponseHistoryDto> CreateAsync(ResponseHistoryDto input);
        Task DeleteAsync(int id);
        Task<ResponseHistoryDto> GetByIdAsync(int id);
        Task<List<ResponseHistoryDto>> GetListAsync();
        Task<ResponseHistoryDto> UpdateAsync(ResponseHistoryDto input);
        Task<List<ResponseHistoryDto>> GetListByOfficeIdAsync(Guid officeId);
    }
}
