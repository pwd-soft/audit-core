using AutoMapper;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace PWD.Audit.Services
{
    public class AssociateAppService : ApplicationService, IAssociateAppService
    {
        private readonly IRepository<Associate, int> _repository;

        public AssociateAppService(IRepository<Associate, int> repository)
        {
            _repository = repository;
        }

        public async Task<List<AssociateDto>> SearchAssociates(AssociateFilter associateFilter)
        {
            var associates = await _repository.GetQueryableAsync();

            if (!String.IsNullOrEmpty(associateFilter.Name))
                associates = associates.Where(x => x.Name.Contains(associateFilter.Name));
            
            if (!String.IsNullOrEmpty(associateFilter.BCSID))
                associates = associates.Where(x => x.BCSID == associateFilter.BCSID);

            return ObjectMapper.Map<IQueryable<Associate>, List<AssociateDto>>(associates);
        }
    }
}
