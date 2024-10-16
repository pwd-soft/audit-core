using PWD.Audit.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IOfficeUserAppService
    {
        Task<OfficeUserDto> CreateAsync(OfficeUserDto input);
        Task DeleteAsync(int id);
        Task<OfficeUserDto> GetByIdAsync(int id);
        Task<List<OfficeUserDto>> GetListAsync();
        Task<OfficeUserDto> UpdateAsync(OfficeUserDto input);
        //Task<List<ObjectionDto>> GetNomineesByEmployeeIdAsync(int employeeId);
    }
}
