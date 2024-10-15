using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IApprovalAppService
    {
        Task<PostingDto> GetPosting(string userName);

        Task<DateTime> LatestOffice();
        Task<List<OrganizationUnitDto>> GetOffices();
        Task<UserInfo> GetUserInfo(string userName);
        Task<UserInfo> GetUserInfoById(string userId);
        Task<PostingDto> GetPostingById(int id);
        Task<List<PostingDto>> GetPostingListById(List<int> ids);
        
    }
}
