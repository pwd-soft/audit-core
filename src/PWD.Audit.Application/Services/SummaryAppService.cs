using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
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
        private readonly IRepository<SummaryLine, int> _summaryLineRepository;
        private IApprovalAppService _approvalAppService;

        private const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";
        public SummaryAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> SummaryLineRepository, IRepository<Objection, int> objectionRepository, IApprovalAppService approvalAppService)
        {
            _repository = repository;
            _summaryLineRepository = SummaryLineRepository;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
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

        public async Task<List<SummaryDto>> AllOfficeSummary()
        {
            var result = new List<SummaryDto>();
            var offices = await _approvalAppService.GetOffices();
            //offices = offices.Where(o => o.civilEm != null).ToList();
            //offices = offices.Where(o => !o.displayName.Contains("P&D")).ToList();

            var auditees = await _approvalAppService.GetUserByRole(AuditMonitoringOfficeRole);
            offices = offices.Where(x => auditees.Contains(x.code)).ToList();
            var ol = new List<OrganizationUnitDto>
            {
                offices.FirstOrDefault(o => o.layer == "Chief")
            };
            offices.Where(o => o.layer == "Zone").ToList().ForEach(z =>
            {
                ol.Add(z);
                var cl = offices.Where(x => x.parentId == z.id).ToList();
                cl.ForEach(c =>
                {
                    ol.Add(c);
                    var dl = offices.Where(x => x.parentId == c.id).ToList();
                    ol.AddRange(dl);
                });
            });
            foreach (var office in ol.Where(x => x != null))
            {
                var summary = await GetByOffice((Guid)office.id);
                summary.OfficeName = office.displayNameBn;
                result.Add(summary);
            }
            return result;
        }

        public async Task<SummaryDto> GetByOffice(Guid officeId)
        {
            var objections = await _objectionRepository.GetListAsync(x=>x.OfficeId==officeId);
            var sfi=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.SFI).ToList();
            var nsfi=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.NonSFI).ToList();
            var dr=objections.Where(o=>o.ObjectionType==Enum.ObjectionType.Draft).ToList();
            var sfiLine = new SummaryLineDto()
            {
                Count = sfi.Count(),
                BroadSheet = sfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = sfi.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = sfi.Count() - sfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = sfi.Sum(x => x.Value),
                Type = Enum.ObjectionType.SFI,
                TypeName = "এসএফআই",
            };
            var nsfiLine = new SummaryLineDto()
            {
                Count = nsfi.Count(),
                BroadSheet = nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = nsfi.Count() - nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = nsfi.Sum(x => x.Value),
                Type = Enum.ObjectionType.NonSFI,
                TypeName = "নন এসএফআই",
            };
            var drLine = new SummaryLineDto()
            {
                Count = dr.Count(),
                BroadSheet = dr.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = dr.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = dr.Count() - dr.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = dr.Sum(x => x.Value),
                Type = Enum.ObjectionType.Draft,
                TypeName = "ড্রাফট",
            };
            var totalLine = new SummaryLineDto()
            {
                Count = objections.Count(),
                BroadSheet = objections.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = objections.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = objections.Count() - objections.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = objections.Sum(x => x.Value),
                Type = Enum.ObjectionType.Draft,
                TypeName = "মোট",
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
