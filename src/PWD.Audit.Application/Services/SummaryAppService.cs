using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
using PWD.Audit.InputDtos;
using PWD.Audit.Interfaces;
using Scriban.Runtime.Accessors;
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
        private IObjectionAppService _objectionAppService;

        private const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";
        public SummaryAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> SummaryLineRepository, IRepository<Objection, int> objectionRepository, IApprovalAppService approvalAppService, IObjectionAppService objectionAppService)
        {
            _repository = repository;
            _summaryLineRepository = SummaryLineRepository;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
            _objectionAppService = objectionAppService;
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
            foreach (var office in offices.Where(x => x != null))
            {
                var summary = await GetByOffice((Guid)office.id);
                summary.OfficeName = office.displayNameBn;
                result.Add(summary);
            }
            return result;
        }

        private async Task<SummaryDto> GetByOffice(Guid officeId)
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

        public async Task<List<SummaryReportDto>> GenerateSummaryReport(SummaryReportInputDto summaryReportCriteria)
        {
            List<SummaryReportDto> SummaryReportData = new List<SummaryReportDto>();

            switch (summaryReportCriteria.Type) 
            {
                case SummaryReportType.PWD:
                    //var lastMonthObjections = objections.Where(o => o.)
                    break;
                case SummaryReportType.Combined:
                    if(summaryReportCriteria.Offices.Count > 0)
                    {
                        //switch (summaryReportCriteria.SubType)
                        //{
                        //    case SummaryReportSubType.Zonewise:
                        //        //summaryReportCriteria.Offices.Any(o => o.id);
                        //        break;
                        //    case SummaryReportSubType.Circlewise:
                        //        break;
                        //}

                        SummaryReportData = await ProcessSummaryData(summaryReportCriteria.Offices, SummaryReportType.Combined);
                    }
                    
                    break;
                case SummaryReportType.Detailed:
                    break;        
                case SummaryReportType.Officewise:
                    break;
            }

            return SummaryReportData;
        }

        private async Task<List<SummaryReportDto>> ProcessSummaryData(List<string> OfficeIds, SummaryReportType type)
        {
            List<SummaryReportDto> Data = new List<SummaryReportDto>();

            var offices = await _approvalAppService.GetOffices();
            var objections = await _objectionAppService.GetListAsync();

            var currentDay = DateTime.Today;
            var firstDayOfCurrentMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

            int serial = 1;

            foreach(var id in OfficeIds)
            {
                var identifyOffice = offices.FirstOrDefault(o => o.id == Guid.Parse(id));
                
                if (identifyOffice is not null) 
                {
                    switch (identifyOffice.layer)
                    {
                        case "Zone":
                            break;
                        case "Circle":
                            break;
                        case "Division":
                            break;
                    }
                }
            }

            switch (type)
            {
                case SummaryReportType.Combined:

                    break;
            }

            return Data;
        }

        private List<SummaryReportDto> GetZoneData(List<ObjectionDto> objections, List<OrganizationUnitDto> offices, SummaryReportType type) 
        {
        }
        
        private List<SummaryReportDto> GetCircleData() 
        {
        }

        private List<SummaryReportDto> GetDivisionData() 
        {
        }
    }
}
