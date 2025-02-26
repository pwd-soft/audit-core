using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IYearlyObjectionAppService
    {
        //Task PopulateYearlyObjections();
        Task<YearlyObjectionDto> GetByIdAsync(int id);
        Task<YearlyObjectionDto> UpdateAsync(YearlyObjectionDto input);
        Task<List<YearlyObjectionDto>> GetListByOfficeCodeAsync(string officeCode);
    }
}
