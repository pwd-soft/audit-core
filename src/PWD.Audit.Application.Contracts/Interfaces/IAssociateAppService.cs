using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IAssociateAppService
    {
        Task<List<AssociateAndObjectionsDto>> SearchAssociates(AssociateFilter associateFilter);
    }
}
