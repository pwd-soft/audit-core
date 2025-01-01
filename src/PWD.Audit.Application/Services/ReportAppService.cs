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
        private readonly IApprovalAppService _approvalAppService;
        private readonly IYearlyObjectionAppService _yearlyObjectionAppService;


        private const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";

        public ReportAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> summaryLineRepository, IRepository<Objection, int> objectionRepository, IObjectionAppService objectionService, IApprovalAppService approvalAppService, IYearlyObjectionAppService yearlyObjectionAppService)
        {
            _repository = repository;
            _summaryLineRepository = summaryLineRepository;
            _objectionService = objectionService;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
            _yearlyObjectionAppService = yearlyObjectionAppService;
        }

        public async Task<GenericListDto<ObjectionReportDto>> DetailedReport(ReportFilterModel reportFilter)
        {
            GenericListDto<ObjectionReportDto> result = new GenericListDto<ObjectionReportDto>();
            List<ObjectionReportDto> processedObjections = new List<ObjectionReportDto>();
            List<OrganizationUnitDto> finalOfficeList = new List<OrganizationUnitDto>();

            var offices = await _approvalAppService.GetOffices();
            var auditees = await _approvalAppService.GetUserByRole(AuditMonitoringOfficeRole);

            offices = offices.Where(x => auditees.Contains(x.code)).ToList();

            if (reportFilter.Offices?.Count > 0)
                finalOfficeList = offices.Where(x => reportFilter.Offices.Contains((Guid)x.id)).ToList();
            else
                finalOfficeList = offices;


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
            var validObjections = new List<ObjectionDto>();
            var objections = await _objectionService.GetListByOfficeIdAsync(officeId);
            if(!objections.Any()) return validObjections;
            var yearlyObjections = await _yearlyObjectionAppService.GetListByOfficeIdAsync(officeId);
            bool flag = false;
            var yearlySum = yearlyObjections.Sum(x => x.NumberOfObjections);

            if (reportFilter.FinancialYear > 0)
            {
                var obj = yearlyObjections
                    .FirstOrDefault(x => x.Year == reportFilter.FinancialYear);
                yearlySum = obj is not null ? obj.NumberOfObjections : 0;
                objections = objections.Where(x => x.Date.Year == reportFilter.FinancialYear).ToList();
            }

            if (objections.Count >= yearlySum)
                validObjections = objections;
            else
                flag = true;

            if (reportFilter.DirectorateType > 0)
                validObjections = validObjections.Where(x => x.DirectorateType == reportFilter.DirectorateType).ToList();

            if (reportFilter.ObjectionType > 0)
                validObjections = validObjections.Where(x => x.ObjectionType == reportFilter.ObjectionType).ToList();

            if (reportFilter.ObjectionStatus > 0)
                validObjections = validObjections.Where(x => x.ObjectionStatus == reportFilter.ObjectionStatus).ToList();

            //if (reportFilter.FinancialYear > 0)
            //    validObjections = validObjections.Where(x => x.Date.Year == reportFilter.FinancialYear).ToList();

            if (flag)
                validObjections.Add(new ObjectionDto { OfficeId = officeId, IsIncomplete = true});

            return validObjections;
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

            if (reportFilter.FinancialYear > 0)
            {
                objectionFilter = objectionFilter.Where(x => x.Date.Year == reportFilter.FinancialYear);
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
