using PWD.Audit.DtoModels;
using PWD.Audit.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Services
{
    public interface IAttachmentAppService
    {
        Task<AttachmentDto> CreateAsync(AttachmentDto input);
        Task DeleteAsync(int id);
        Task<AttachmentDto> GetByIdAsync(int id);
        Task<List<AttachmentDto>> GetListByObjectionIdAsync(int id, AttachmentType type);
        Task<List<AttachmentDto>> GetListResponseIdAsync(int id, AttachmentType type);
        Task<AttachmentDto> UpdateAsync(AttachmentDto input);
        Task InsertBulkAsync(IEnumerable<AttachmentDto> newAttachments);
        Task<List<AttachmentSummaryDto>> GetObjectionAttachmentCount(List<int> ids, AttachmentType type);

        //Task<List<AttachmentDto>> GetListAsync();
        //Task<List<AttachmentDto>> GetListByObjectionIdAsync(int objectionId);
        //Task<ResponseStateDto> UpdateResponseStateAsync(ResponseStateDto responseStateDto);
    }
}