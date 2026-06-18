using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AssociateAppService : ApplicationService, IAssociateAppService
    {
        private readonly IRepository<Associate, int> _repository;
        private readonly IRepository<Objection, int> _objectionRepository;

        public AssociateAppService(IRepository<Associate, int> repository, IRepository<Objection, int> objectionRepository)
        {
            _repository = repository;
            _objectionRepository = objectionRepository;
        }

        public async Task<List<AssociateAndObjectionsDto>> SearchAssociates(AssociateFilter associateFilter)
        {
            var associates = await _repository.GetQueryableAsync();

            if (!String.IsNullOrEmpty(associateFilter.Name.Trim()))
                associates = associates.Where(x => x.Name.Contains(associateFilter.Name.Trim()));
            
            if (!String.IsNullOrEmpty(associateFilter.BCSID.Trim()))
                associates = associates.Where(x => x.BCSID == associateFilter.BCSID.Trim());
            
            var associateAndObjections = new List<AssociateAndObjectionsDto>();

            var aDto = ObjectMapper.Map<IQueryable<Associate>, List<AssociateDto>>(associates);

            var bids=aDto.Where(x=>x.BCSID?.Length>1).Select(x => x.BCSID).Distinct();
            
            foreach (var id in bids)
            {
                var aoDto = new AssociateAndObjectionsDto();
                aoDto.Associate = aDto.First(x => x.BCSID == id);
                
                var objectionIds=aDto.Where(x => x.BCSID == id).Select(x=>x.ObjectionId);
                var objections=await _objectionRepository.GetListAsync(x => objectionIds.Contains(x.Id));
                aoDto.Objections = ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objections);

                associateAndObjections.Add(aoDto);
            }

            aDto = aDto.Where(x => x.BCSID == null || x.BCSID?.Length == 0).ToList();

            var names=aDto.Select(x => x.Name).Distinct();

            foreach (var name in names)
            {
                var aoDto = new AssociateAndObjectionsDto();
                aoDto.Associate = aDto.First(x => x.Name == name);

                var objectionIds = aDto.Where(x => x.Name == name).Select(x => x.ObjectionId);
                var objections = await _objectionRepository.GetListAsync(x => objectionIds.Contains(x.Id));
                aoDto.Objections = ObjectMapper.Map<List<Objection>, List<ObjectionDto>>(objections);

                associateAndObjections.Add(aoDto);
            }
            associateAndObjections.ForEach(a => a.Objections.ForEach(o => o.Associates = []));
            //foreach (var association in associates.ToList()) 
            //{
            //    associateAndObjections.Add(
            //        new AssociateAndObjectionsDto() 
            //        {
            //            Associate = ObjectMapper.Map<Associate, AssociateDto>(association),

            //        }
            //    );
            //}

            return associateAndObjections; // ObjectMapper.Map<IQueryable<Associate>, List<AssociateDto>>(associates);
        }
    }
}
