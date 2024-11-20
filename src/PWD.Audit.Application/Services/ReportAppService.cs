using Microsoft.EntityFrameworkCore;
using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
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

        public async Task<GenericListDto<OrganizationUnitDto>> SummaryReport(ReportFilterModel reportFilter)
        {
            GenericListDto<OrganizationUnitDto> result = new GenericListDto<OrganizationUnitDto>();
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

            //var test = await GetByOffice(new Guid("82411264-c6b5-42e7-b750-085ff676b09f"), reportFilter);

            foreach (var office in ol.Where(x => x != null))
            {
                var summary = await GetByOffice((Guid)office.id, reportFilter);
                summary.OfficeName = office.displayNameBn;
                result.ListData.Add(summary);
            }
            
            result.CountData = ol.Where(x => x != null).Count();
            ol = ol.AsEnumerable()
                //.OrderByDescending(o => o.CreationTime)
                .Skip(reportFilter.Offset)
                .Take(reportFilter.Limit).ToList();

            result.ListData = ol;

            return result;
        }

        private async Task<SummaryDto> GetByOffice(Guid officeId, ReportFilterModel reportFilter)
        {
            var objectionFilter = await _objectionRepository.GetQueryableAsync();
            objectionFilter = objectionFilter.Where(x => x.OfficeId == officeId);

            //if (reportFilter.ReportType > 0)
            //{
            //    switch (reportFilter.ReportType)
            //    {
            //        case ReportType.Resolved:
            //            objectionFilter = objectionFilter.Where(x => x.IsResolved);
            //            break;
            //        default:
            //            objectionFilter = objectionFilter.Where(x => x.ObjectionType == reportFilter.ObjectionType);
            //            break;
            //    }
            //}

            if (reportFilter.DirectorateType > 0 )
            {
                objectionFilter = objectionFilter.Where(x => x.DirectorateType == reportFilter.DirectorateType);
            }

            if (reportFilter.ObjectionType > 0)
            {
                objectionFilter = objectionFilter.Where(x => x.ObjectionType == reportFilter.ObjectionType);
            }

            if (reportFilter.ObjectionStatus > 0)
            {
                objectionFilter = objectionFilter.Where(x => x.ObjectionStatus == reportFilter.ObjectionStatus);
            }

            if (!reportFilter.FinancialYear.IsNullOrEmpty())
            {
                objectionFilter = objectionFilter.Where(x => x.FinancialYear == reportFilter.FinancialYear);
            }

            //if (reportFilter.Offices?.Count > 0) 
            //{
            //    objectionFilter = objectionFilter.Where(x => EF.Functions.IsIn((IEnumerable<DbFunctions>)reportFilter.Offices));
            //}


            var finalList = objectionFilter.ToList();

            var objections = await _objectionRepository.GetListAsync(x => x.OfficeId == officeId);
            var sfi = objections.Where(o => o.ObjectionType == Enum.ObjectionType.SFI).ToList();
            var nsfi = objections.Where(o => o.ObjectionType == Enum.ObjectionType.NonSFI).ToList();
            var dr = objections.Where(o => o.ObjectionType == Enum.ObjectionType.Draft).ToList();
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
    }
}
