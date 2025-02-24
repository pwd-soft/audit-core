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
            var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

            int serial = 1;

            foreach(var id in OfficeIds)
            {
                var result = GetZoneData(objections, offices, Guid.Parse(id), type);
                Data.AddRange(result);
            }

            //switch (type)
            //{
            //    case SummaryReportType.Combined:

            //        break;
            //}

            return Data;
        }

        private List<SummaryReportDto> GetZoneData(List<ObjectionDto> objections, List<OrganizationUnitDto> offices, Guid officeId, SummaryReportType type)
        {
            List<SummaryReportDto> zoneSummaryList = new List<SummaryReportDto>();
            var objectionList = objections.Where(o => o.OfficeId == officeId).ToList();
            var summary = AssignData(objectionList);
            zoneSummaryList.Add(summary);
            
            var circleSummary = GetCircleData(objections, offices, officeId, type);
            
            if(type == SummaryReportType.Combined) 
            {
                zoneSummaryList = CombineData(zoneSummaryList, circleSummary);
            }
            
            if(type == SummaryReportType.Detailed) 
            {
                zoneSummaryList.AddRange(circleSummary);
            }
            
            return zoneSummaryList;
        }

        private List<SummaryReportDto> GetCircleData(List<ObjectionDto> objections, List<OrganizationUnitDto> offices, Guid officeId, SummaryReportType type)
        {
            List<SummaryReportDto> circleSummaryList = new List<SummaryReportDto>();
            var circles = offices.Where(o => o.parentId == officeId).ToList();

            foreach (var item in circles)
            {
                var objectionList = objections.Where(o => o.OfficeId == item.id).ToList();
                var summary = AssignData(objectionList);
                circleSummaryList.Add(summary);

                var divisionSummary = GetDivisionData(objections, offices, officeId, type);

                if (type == SummaryReportType.Combined)
                {
                    circleSummaryList = CombineData(circleSummaryList, divisionSummary);
                }

                if (type == SummaryReportType.Detailed)
                {
                    circleSummaryList.AddRange(divisionSummary);
                }
            }           
            
            return circleSummaryList;
        }

        private List<SummaryReportDto> GetDivisionData(List<ObjectionDto> objections, List<OrganizationUnitDto> offices, Guid officeId, SummaryReportType type)
        {
            List<SummaryReportDto> divisionSummaryList = new List<SummaryReportDto>();
            var circles = offices.Where(o => o.parentId == officeId).ToList();

            foreach (var item in circles)
            {
                var objectionList = objections.Where(o => o.OfficeId == item.id).ToList();
                var summary = AssignData(objectionList);
                divisionSummaryList.Add(summary);

                var divisionSummary = GetDivisionData(objections, offices, officeId, type);

                if (type == SummaryReportType.Combined)
                {
                    divisionSummaryList = CombineData(divisionSummaryList, divisionSummary);
                }

                if (type == SummaryReportType.Detailed)
                {
                    divisionSummaryList.AddRange(divisionSummary);
                }
            }

            return divisionSummaryList;
        }

        private SummaryReportDto AssignData(List<ObjectionDto> list) 
        {
            var currentDay = DateTime.Today;
            var firstDayOfCurrentMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
            var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

            var previousMonthData = list.Where(l => l.ObjectionDate <= lastDayOfPreviousMonth);
            var currentMonthData = list.Where(l => l.ObjectionDate >= firstDayOfCurrentMonth);

            var summary = new SummaryReportDto();

            summary.PreviousObjectionNumber = previousMonthData.Count();
            summary.PreviousObjectionAmount = previousMonthData.Sum(p => p.Value);

            summary.CurrentObjectionNumber = currentMonthData.Count();
            summary.CurrentObjectionAmount = currentMonthData.Sum(p => p.Value);

            summary.SubTotalObjectionNumber = summary.PreviousObjectionNumber + summary.CurrentObjectionNumber;
            summary.SubTotalObjectionAmount = summary.PreviousObjectionAmount + summary.CurrentObjectionAmount;

            //summary.PreviousBroadsheetNumber = previousMonthData.Count(p => p.ObjectionDate >= firstDayOfPreviousMonth && p.ObjectionDate <= lastDayOfPreviousMonth);
            //summary.UnsetteledBroadsheetNumber = previousMonthData.Count(p => p.ObjectionDate >= firstDayOfPreviousMonth && p.ObjectionDate <= lastDayOfPreviousMonth);

            summary.CurrentObjectionSettlementNumber = currentMonthData.Count(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay);
            summary.CurrentObjectionSettlementAmount = currentMonthData.Where(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay).Sum(s => s.Value);

            summary.NonSfiNumber = list.Count(l => l.ObjectionType == ObjectionType.NonSFI) - list.Count(l => l.ObjectionType == ObjectionType.NonSFI && l.MemoNumber is not null);
            summary.SfiNumber = list.Count(l => l.ObjectionType == ObjectionType.SFI) - list.Count(l => l.ObjectionType == ObjectionType.SFI && l.MemoNumber is not null);
            summary.DraftNumber = list.Count(l => l.ObjectionType == ObjectionType.Draft) - list.Count(l => l.ObjectionType == ObjectionType.Draft && l.MemoNumber is not null);
            summary.TotalObjectionNumber = summary.NonSfiNumber + summary.SfiNumber + summary.DraftNumber;
            summary.TotalObjectionAmount = list.Where(l => l.MemoNumber == null).Sum(s => s.Value);

            return summary;
        }
        
        private List<SummaryReportDto> CombineData(List<SummaryReportDto> destination, List<SummaryReportDto> source) 
        {
            foreach (var item in source)
            {
                destination[0].PreviousObjectionNumber += item.PreviousObjectionNumber;
                destination[0].PreviousObjectionAmount += item.PreviousObjectionAmount;
                destination[0].CurrentObjectionNumber += item.CurrentObjectionNumber;
                destination[0].CurrentObjectionAmount += item.CurrentObjectionAmount;
                destination[0].SubTotalObjectionNumber += item.SubTotalObjectionNumber;
                destination[0].SubTotalObjectionAmount += item.SubTotalObjectionAmount;
                destination[0].PreviousBroadsheetNumber += item.PreviousBroadsheetNumber;
                destination[0].UnsetteledBroadsheetNumber += item.UnsetteledBroadsheetNumber;
                destination[0].CurrentObjectionSettlementNumber += item.CurrentObjectionSettlementNumber;
                destination[0].CurrentObjectionSettlementAmount += item.CurrentObjectionSettlementAmount;
                destination[0].NonSfiNumber += item.NonSfiNumber;
                destination[0].SfiNumber += item.SfiNumber;
                destination[0].DraftNumber += item.DraftNumber;
                destination[0].TotalObjectionNumber += item.TotalObjectionNumber;
                destination[0].TotalObjectionAmount += item.TotalObjectionAmount;

            }

            return destination;
        }
    }
}
