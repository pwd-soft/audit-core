using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Attendance_Swagger.Interfaces
{
    public interface IObjectionAppService
    {
        Task<ObjectionDto> CreateAsync(ObjectionDto objectionInput);
        Task DeleteAsync(int id);
        Task<ObjectionDto> GetByIdAsync(int id);
        Task<List<ObjectionDto>> GetListAsync();
        Task<ObjectionDto> UpdateAsync(ObjectionDto objectionInput);
        //Task<List<ObjectionDto>> GetNomineesByEmployeeIdAsync(int employeeId);
    }
}
