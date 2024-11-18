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

namespace PWD.Audit.Services
{
    public class ReportAppService : ApplicationService, IReportAppService
    {
        private readonly IRepository<Summary, int> _repository;
        private readonly IRepository<Objection, int> _objectionRepository;
        private readonly IRepository<SummaryLine, int> _summaryLineRepository;
        private IApprovalAppService _approvalAppService;

        public ReportAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> SummaryLineRepository, IRepository<Objection, int> objectionRepository, IApprovalAppService approvalAppService)
        {
            _repository = repository;
            _summaryLineRepository = SummaryLineRepository;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
        }

        public async Task<List<SummaryDto>> SummaryReport(ReportFilterModel reportFilter)
        {
            var result = new List<SummaryDto>();
            var offices = await _approvalAppService.GetOffices();
            offices = offices.Where(o => o.civilEm != null).ToList();
            offices = offices.Where(o => !o.displayName.Contains("P&D")).ToList();

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

            var test = await GetByOffice((Guid)ol[2].id, reportFilter);

            //foreach (var office in ol.Where(x => x != null))
            //{
            //    var summary = await GetByOffice((Guid)office.id, reportFilter);
            //    summary.OfficeName = office.displayNameBn;
            //    result.Add(summary);
            //}

            return result;
        }

        private async Task<SummaryDto> GetByOffice(Guid officeId, ReportFilterModel reportFilter)
        {
            var objectionFilter = await _objectionRepository.GetQueryableAsync();
            objectionFilter = objectionFilter.Where(x => x.OfficeId == officeId);

            if (reportFilter.DirectorateType > 0 )
            {
                objectionFilter = objectionFilter.Where(x => x.DirectorateType == (reportFilter.DirectorateType));
            }

            if (reportFilter.ObjectionType > 0)
            {
                objectionFilter = objectionFilter.Where(x => x.ObjectionType == (reportFilter.ObjectionType));
            }

            if (reportFilter.ReportType > 0)
            {
                switch (reportFilter.ReportType)
                {
                    //case Enum.ReportType.Resolved:
                    //    objectionFilter = objectionFilter.Where(x => x.IsResolved);
                    //    break;
                    default:
                        objectionFilter = objectionFilter.Where(x => x.ObjectionType == reportFilter.ObjectionType);
                        break;
                }
            }

            var finalList = objectionFilter.ToList();

            var objections = await _objectionRepository.GetListAsync(x => x.OfficeId == officeId);
            var sfi = objections.Where(o => o.ObjectionType == Enum.ObjectionType.SFI).ToList();
            var nsfi = objections.Where(o => o.ObjectionType == Enum.ObjectionType.NonSFI).ToList();
            var dr = objections.Where(o => o.ObjectionType == Enum.ObjectionType.Draft).ToList();
            //var sfiLine = new SummaryLineDto()
            //{
            //    Count = sfi.Count(),
            //    BroadSheet = sfi.Count(x => x.IsBroadSheet),
            //    Resolved = sfi.Count(x => x.IsResolved),
            //    NonBroadSheet = sfi.Count() - sfi.Count(x => x.IsBroadSheet),
            //    Value = sfi.Sum(x => x.Value),
            //    Type = Enum.ObjectionType.SFI,
            //    TypeName = "এসএফআই",
            //};
            //var nsfiLine = new SummaryLineDto()
            //{
            //    Count = nsfi.Count(),
            //    BroadSheet = nsfi.Count(x => x.IsBroadSheet),
            //    Resolved = nsfi.Count(x => x.IsResolved),
            //    NonBroadSheet = nsfi.Count() - nsfi.Count(x => x.IsBroadSheet),
            //    Value = nsfi.Sum(x => x.Value),
            //    Type = Enum.ObjectionType.NonSFI,
            //    TypeName = "নন এসএফআই",
            //};
            //var drLine = new SummaryLineDto()
            //{
            //    Count = dr.Count(),
            //    BroadSheet = dr.Count(x => x.IsBroadSheet),
            //    Resolved = dr.Count(x => x.IsResolved),
            //    NonBroadSheet = dr.Count() - dr.Count(x => x.IsBroadSheet),
            //    Value = dr.Sum(x => x.Value),
            //    Type = Enum.ObjectionType.Draft,
            //    TypeName = "ড্রাফট",
            //};
            //var totalLine = new SummaryLineDto()
            //{
            //    Count = objections.Count(),
            //    BroadSheet = objections.Count(x => x.IsBroadSheet),
            //    Resolved = objections.Count(x => x.IsResolved),
            //    NonBroadSheet = objections.Count() - objections.Count(x => x.IsBroadSheet),
            //    Value = objections.Sum(x => x.Value),
            //    Type = Enum.ObjectionType.Draft,
            //    TypeName = "মোট",
            //};
            var result = new SummaryDto() { OfficeId = officeId };
            //result.SummaryLines.Add(sfiLine);
            //result.SummaryLines.Add(nsfiLine);
            //result.SummaryLines.Add(drLine);
            //result.SummaryLines.Add(totalLine);
            return result;
        }
    }
}
