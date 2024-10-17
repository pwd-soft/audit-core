using PWD.Audit.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface ISummaryAppService
    {
        Task<SummaryDto> CreateAsync(SummaryDto input);
        Task DeleteAsync(int id);
        Task<List<SummaryDto>> GetListAsync();
        Task<SummaryDto> UpdateAsync(SummaryDto input);
    }
}
