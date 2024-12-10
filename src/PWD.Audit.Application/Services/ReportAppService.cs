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
        private readonly IObjectionAppService _objectionService;
        private readonly IRepository<Objection, int> _objectionRepository;
        private readonly IRepository<SummaryLine, int> _summaryLineRepository;
        private IApprovalAppService _approvalAppService;

        private const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";

        public ReportAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> summaryLineRepository, IRepository<Objection, int> objectionRepository, IObjectionAppService objectionService, IApprovalAppService approvalAppService)
        {
            _repository = repository;
            _summaryLineRepository = summaryLineRepository;
            _objectionService = objectionService;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
        }

        public async Task<GenericListDto<ObjectionReportDto>> DetailedReport(ReportFilterModel reportFilter)
        {
            GenericListDto<ObjectionReportDto> result = new GenericListDto<ObjectionReportDto>();
            //List<SummaryDto> processedSummaries = new List<SummaryDto>();
            List<ObjectionReportDto> processedObjections = new List<ObjectionReportDto>();
            List<OrganizationUnitDto> finalOfficeList = new List<OrganizationUnitDto>();

            var offices = await _approvalAppService.GetOffices();
            var auditees = await _approvalAppService.GetUserByRole(AuditMonitoringOfficeRole);

            offices = offices.Where(x => auditees.Contains(x.code)).ToList();

            if (reportFilter.Offices?.Count > 0)
            {
                finalOfficeList = offices.Where(x => reportFilter.Offices.Contains((Guid)x.id)).ToList();
            }
            else
            {
                //var ol = new List<OrganizationUnitDto>
                //{
                //    offices.FirstOrDefault(o => o.layer == "Chief")
                //};

                //offices.Where(o => o.layer == "Zone").ToList().ForEach(z =>
                //{
                //    ol.Add(z);
                //    var cl = offices.Where(x => x.parentId == z.id).ToList();
                //    cl.ForEach(c =>
                //    {
                //        ol.Add(c);
                //        var dl = offices.Where(x => x.parentId == c.id).ToList();
                //        ol.AddRange(dl);
                //    });
                //});
                //finalOfficeList = ol;
                //finalOfficeList = finalOfficeList.Where(x => x != null).ToList();
                finalOfficeList = offices;
            }

            foreach (var office in finalOfficeList) 
            {
                ObjectionReportDto objectionReport = new ObjectionReportDto();
                objectionReport.OrgUnit = office;
                objectionReport.Objections = await GetObjectionsByOffice((Guid)office.id, reportFilter);
                processedObjections.Add(objectionReport);
            }

            result.CountData = processedObjections.Count();
            processedObjections = processedObjections.AsEnumerable()
                .Skip(reportFilter.Offset)
                .Take(reportFilter.Limit).ToList();

            result.ListData = processedObjections;

            return result;
        }

        private async Task<List<ObjectionDto>> GetObjectionsByOffice(Guid officeId, ReportFilterModel reportFilter) 
        {
            var objections = await _objectionService.GetListByOfficeIdAsync((Guid)officeId);

            if (reportFilter.DirectorateType > 0)
            {
                objections = objections.Where(x => x.DirectorateType == reportFilter.DirectorateType).ToList();
            }

            if (reportFilter.ObjectionType > 0)
            {
                objections = objections.Where(x => x.ObjectionType == reportFilter.ObjectionType).ToList();
            }

            if (reportFilter.ObjectionStatus > 0)
            {
                objections = objections.Where(x => x.ObjectionStatus == reportFilter.ObjectionStatus).ToList();
            }

            if (!reportFilter.FinancialYear.IsNullOrEmpty())
            {
                objections = objections.Where(x => x.FinancialYear == reportFilter.FinancialYear).ToList();
            }

            return objections;
        }

        private async Task<SummaryDto> GetSummaryByOffice(Guid officeId, ReportFilterModel reportFilter)
        {
            var objectionFilter = await _objectionRepository.GetQueryableAsync();
            objectionFilter = objectionFilter.Where(x => x.OfficeId == officeId);

            if (reportFilter.DirectorateType > 0)
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
                Type = Enum.ObjectionType.None,
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
