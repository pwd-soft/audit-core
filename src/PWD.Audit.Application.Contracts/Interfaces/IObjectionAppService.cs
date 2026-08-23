using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IObjectionAppService
    {
        Task<ObjectionDto> CreateAsync(ObjectionDto input);
        Task DeleteAsync(int id);
        Task<ObjectionDto> GetDetailsByIdAsync(int id);
        Task<ObjectionDto> GetByIdAsync(int id);
        Task<List<ObjectionDto>> GetListAsync();
        Task<ObjectionDto> UpdateAsync(ObjectionDto input);
        Task<List<ObjectionDto>> GetListByOfficeCodeAsync(string officeCode);
        Task<List<ObjectionDto>> GetIncomingResponseListAsync(string officeCode);
        Task<ObjectionDto> UpdateObjectionStatusAsync(ObjectionDto input);
        Task<GenericListDto<ObjectionDto>> GetIncomingResponseListWithPagingAsync(ResponseFilterModel filterCriteria);
    }    
}
