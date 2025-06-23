using PWD.Audit.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IResponseCommentAppService
    {
        Task<ResponseCommentDto> CreateAsync(ResponseCommentDto input);
        Task<ResponseCommentDto> GetByIdAsync(int id);
        Task<List<ResponseCommentDto>> GetListAsync();
        Task<ResponseCommentDto> UpdateAsync(ResponseCommentDto input);
        Task<ResponseCommentDto> GetSuperiorLevelComment(int responseHistoryId, string superiorOfficeCode);
    }    
}
