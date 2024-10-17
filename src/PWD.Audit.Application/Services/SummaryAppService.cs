using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PWD.Audit.Services
{
    public class SummaryAppService : ApplicationService, ISummaryAppService
    {
        private readonly IRepository<Summary, int> _repository;
        private readonly IRepository<Objection, int> _objectionRepository;
        private readonly IRepository<SummaryLine, int> _SummaryLineRepository;

        public SummaryAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> SummaryLineRepository, IRepository<Objection, int> objectionRepository)
        {
            _repository = repository;
            _SummaryLineRepository = SummaryLineRepository;
            _objectionRepository = objectionRepository;
        }

        public async Task<SummaryDto> CreateAsync(SummaryDto SummaryInput)
        {
            var Summary = ObjectMapper.Map<SummaryDto, Summary>(SummaryInput);
            var newSummary = await _repository.InsertAsync(Summary, true);

            return ObjectMapper.Map<Summary, SummaryDto>(newSummary);
        }

        public async Task<SummaryDto> UpdateAsync(SummaryDto SummaryInput)
        {
            var dbItem = await _repository.GetAsync(SummaryInput.Id);

            if (dbItem is not null)
            { 
            }

            var updatedItem = await _repository.UpdateAsync(dbItem);
            return ObjectMapper.Map<Summary, SummaryDto>(updatedItem);
        }

        public async Task<SummaryDto> GetByOffice(Guid officeId)
        {
            var objections = await _objectionRepository.GetListAsync(x=>x.OfficeId==officeId);
            var sfi=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.SFI).ToList();
            var nsfi=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.NonSFI).ToList();
            var dr=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.Draft).ToList();
            var sfiLine=new SummaryLineDto()
            {
                Count=sfi.Count() ,
                BroadSheet=sfi.Count(x=>x.IsBroadSheet) ,
                Resolved=sfi.Count(x=>x.IsResolved) ,
                NonBroadSheet= sfi.Count()- sfi.Count(x => x.IsBroadSheet),
                Value=sfi.Sum(x=>x.Value) ,
                Type=Enum.ObjectionType.SFI,
                TypeName= "এসএফআই",
            };
            var nsfiLine=new SummaryLineDto()
            {
                Count= nsfi.Count() ,
                BroadSheet= nsfi.Count(x=>x.IsBroadSheet) ,
                Resolved= nsfi.Count(x=>x.IsResolved) ,
                NonBroadSheet= nsfi.Count()- nsfi.Count(x => x.IsBroadSheet),
                Value= nsfi.Sum(x=>x.Value) ,
                Type=Enum.ObjectionType.NonSFI,
                TypeName= "নন এসএফআই",
            };
            var drLine=new SummaryLineDto()
            {
                Count= dr.Count() ,
                BroadSheet= dr.Count(x=>x.IsBroadSheet) ,
                Resolved= dr.Count(x=>x.IsResolved) ,
                NonBroadSheet= dr.Count()- dr.Count(x => x.IsBroadSheet),
                Value= dr.Sum(x=>x.Value) ,
                Type=Enum.ObjectionType.Draft,
                TypeName= "ড্রাফট",
            };
            var totalLine=new SummaryLineDto()
            {
                Count= objections.Count() ,
                BroadSheet= objections.Count(x=>x.IsBroadSheet) ,
                Resolved= objections.Count(x=>x.IsResolved) ,
                NonBroadSheet= objections.Count()- objections.Count(x => x.IsBroadSheet),
                Value= objections.Sum(x=>x.Value) ,
                Type=Enum.ObjectionType.Draft,
                TypeName= "মোট",
            };
            var result = new SummaryDto() { OfficeId = officeId };
            result.SummaryLines.Add(sfiLine);
            result.SummaryLines.Add(nsfiLine);
            result.SummaryLines.Add(drLine);
            result.SummaryLines.Add(totalLine);
            return result;
        }

        public async Task<List<SummaryDto>> GetListAsync() => ObjectMapper.Map<List<Summary>, List<SummaryDto>>(await _repository.GetListAsync());

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

       
    }
}
