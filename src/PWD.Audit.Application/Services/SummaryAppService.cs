using PWD.Audit.DtoModels;
using PWD.Audit.Entities;
using PWD.Audit.Enum;
using PWD.Audit.InputDtos;
using PWD.Audit.Interfaces;
using PWD.Audit.Models;
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

        private IRepository<ResponseHistory, int> _responseHistory;
        private IRepository<YearlyObjection, int> _yearlyObjection;

        private const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";
        private List<OrganizationUnitDto> offices = new List<OrganizationUnitDto>();

        public SummaryAppService(IRepository<Summary, int> repository, IRepository<SummaryLine, int> SummaryLineRepository, IRepository<Objection, int> objectionRepository, IApprovalAppService approvalAppService, IObjectionAppService objectionAppService
            , IRepository<ResponseHistory, int> responseHistory, IRepository<YearlyObjection, int> yearlyObjection, IRepository<Objection, int> objectionHistory)
        {
            _repository = repository;
            _summaryLineRepository = SummaryLineRepository;
            _objectionRepository = objectionRepository;
            _approvalAppService = approvalAppService;
            _objectionAppService = objectionAppService;

            _responseHistory = responseHistory;
            _yearlyObjection = yearlyObjection;
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
                var summary = await GetByOffice(office.code);
                summary.OfficeName = office.displayNameBn;
                result.Add(summary);
            }
            return result;
        }

        public async Task<SummaryDto> GetByOffice(string officeCode)
        {
            var objections = await _objectionRepository.GetListAsync(x => x.OfficeCode == officeCode);
            var sfi = objections.Where(o => o.ObjectionType == ObjectionType.SFI).ToList();
            var nsfi = objections.Where(o => o.ObjectionType == ObjectionType.NonSFI).ToList();
            var dr = objections.Where(o => o.ObjectionType == ObjectionType.Draft).ToList();
            var sfiLine = new SummaryLineDto()
            {
                Count = sfi.Count(),
                BroadSheet = sfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = sfi.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = sfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = sfi.Sum(x => x.Value),
                Type = ObjectionType.SFI,
                TypeName = "এসএফআই",
            };
            var nsfiLine = new SummaryLineDto()
            {
                Count = nsfi.Count(),
                BroadSheet = nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = nsfi.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = nsfi.Sum(x => x.Value),
                Type = ObjectionType.NonSFI,
                TypeName = "নন এসএফআই",
            };
            var drLine = new SummaryLineDto()
            {
                Count = dr.Count(),
                BroadSheet = dr.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = dr.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = dr.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = dr.Sum(x => x.Value),
                Type = ObjectionType.Draft,
                TypeName = "ড্রাফট",
            };
            var totalLine = new SummaryLineDto()
            {
                Count = objections.Count(),
                BroadSheet = objections.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetAnswered),
                Resolved = objections.Count(x => x.ObjectionStatus == ObjectionStatus.Resolved),
                NonBroadSheet = objections.Count(x => x.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered),
                Value = objections.Sum(x => x.Value),
                Type = Enum.ObjectionType.Draft,
                TypeName = "মোট",
            };
            var result = new SummaryDto() { OfficeCode = officeCode };
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
            offices = await _approvalAppService.GetOffices();

            switch (summaryReportCriteria.Type)
            {
                case SummaryReportType.PWD:
                //if (summaryReportCriteria.Offices.Count > 0)
                //{
                //    SummaryReportData = await ProcessSummaryData(summaryReportCriteria.Offices, summaryReportCriteria.Type);
                //}
                //break;
                case SummaryReportType.Combined:
                    if (summaryReportCriteria.Offices.Count > 0)
                    {
                        SummaryReportData = await ProcessSummaryData(summaryReportCriteria.Offices, summaryReportCriteria.Type);
                    }
                    break;
                case SummaryReportType.Detailed:
                    if (summaryReportCriteria.Offices.Count > 0)
                    {
                        SummaryReportData = await ProcessSummaryData(summaryReportCriteria.Offices, summaryReportCriteria.Type);
                    }
                    break;
                case SummaryReportType.Officewise:
                    SummaryReportData = await GetOfficeData(summaryReportCriteria.Offices);
                    break;
            }

            return SummaryReportData;
        }

        private async Task<List<SummaryReportDto>> ProcessSummaryData(List<string> OfficeIds, SummaryReportType type)
        {
            List<SummaryReportDto> Data = new List<SummaryReportDto>();

            int serial = 1;

            foreach (var id in OfficeIds)
            {
                var result = await GetZoneData(id, type);
                Data.AddRange(result);
            }

            foreach (var item in Data)
            {
                item.Serial = serial;
                serial++;
            }

            return Data;
        }

        private async Task<List<SummaryReportDto>> GetZoneData(string officeCode, SummaryReportType type)
        {
            List<SummaryReportDto> zoneSummaryList = new List<SummaryReportDto>();
            var objectionList = await _objectionAppService.GetListByOfficeCodeAsync(officeCode);
            var summary = AssignData(objectionList);
            summary.Name = offices.FirstOrDefault(o => o.code == officeCode).displayNameBn;
            zoneSummaryList.Add(summary);

            var circleSummary = await GetCircleData(officeCode, type);

            if (type == SummaryReportType.Combined)
            {
                zoneSummaryList = CombineData(zoneSummaryList, circleSummary);
            }

            if (type == SummaryReportType.Detailed)
            {
                zoneSummaryList.AddRange(circleSummary);
            }

            return zoneSummaryList;
        }

        private async Task<List<SummaryReportDto>> GetCircleData(string officeCode, SummaryReportType type)
        {
            List<SummaryReportDto> circleSummaryList = new List<SummaryReportDto>();
            var circles = offices.Where(o => o.parentCode == officeCode).ToList();

            foreach (var item in circles)
            {
                var objectionList = await _objectionAppService.GetListByOfficeCodeAsync(item.code);
                var summary = AssignData(objectionList);
                summary.Name = item.displayNameBn;
                circleSummaryList.Add(summary);

                var divisionSummary = await GetDivisionData(item.code, type);

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

        private async Task<List<SummaryReportDto>> GetDivisionData(string officeCode, SummaryReportType type)
        {
            List<SummaryReportDto> divisionSummaryList = new List<SummaryReportDto>();
            var circles = offices.Where(o => o.parentCode == officeCode).ToList();

            foreach (var item in circles)
            {
                var objectionList = await _objectionAppService.GetListByOfficeCodeAsync(item.code);
                var summary = AssignData(objectionList);
                summary.Name = item.displayNameBn;

                if (type == SummaryReportType.Combined)
                {
                    var divisionSummary = new List<SummaryReportDto>();
                    divisionSummary.Add(summary);
                    divisionSummaryList = CombineData(divisionSummaryList, divisionSummary);
                }

                if (type == SummaryReportType.Detailed)
                {
                    divisionSummaryList.Add(summary);
                }
            }

            return divisionSummaryList;
        }

        private async Task<List<SummaryReportDto>> GetOfficeData(List<string> OfficeIds)
        {
            var officeSummaryList = new List<SummaryReportDto>();
            foreach (var item in OfficeIds)
            {
                var objections = await _objectionAppService.GetListByOfficeCodeAsync(item);
                var summary = AssignData(objections);
                summary.Name = offices.FirstOrDefault(o => o.code == item).displayNameBn;
                officeSummaryList.Add(summary);
            }

            return officeSummaryList;
        }

        private SummaryReportDto AssignData(List<ObjectionDto> list)
        {
            var currentDay = DateTime.Today;
            var firstDayOfCurrentMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
            var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

            var previousMonthData = list.Where(l => l.ObjectionDate >= firstDayOfPreviousMonth && l.ObjectionDate <= lastDayOfPreviousMonth).ToList();
            var currentMonthData = list.Where(l => l.ObjectionDate >= firstDayOfCurrentMonth).ToList();

            var summary = new SummaryReportDto();

            summary.PreviousObjectionNumber = previousMonthData.Count();
            summary.PreviousObjectionAmount = previousMonthData.Sum(p => p.Value);

            summary.CurrentObjectionNumber = currentMonthData.Count();
            summary.CurrentObjectionAmount = currentMonthData.Sum(p => p.Value);

            summary.SubTotalObjectionNumber = summary.PreviousObjectionNumber + summary.CurrentObjectionNumber;
            summary.SubTotalObjectionAmount = summary.PreviousObjectionAmount + summary.CurrentObjectionAmount;

            summary.PreviousBroadsheetNumber = previousMonthData.Count(p => p.ObjectionStatus == ObjectionStatus.BroadSheetAnswered);
            summary.UnsetteledBroadsheetNumber = list.Count - list.Count(l => l.ObjectionStatus == ObjectionStatus.Resolved) - list.Count(l => l.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered);

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
            if (destination.Count == 0)
                destination.Add(new SummaryReportDto());
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

        //public async Task UpdateOfficeCodes(SummaryReportInputDto summaryReportCriteria)
        //{
        //    offices = await _approvalAppService.GetOffices();

        //    //var objections = await _objectionRepository.GetListAsync();
        //    //foreach (var item in objections)
        //    //{
        //    //    item.OfficeCode = offices.FirstOrDefault(o => o.id == item.OfficeId).code;
        //    //}
        //    //await _objectionRepository.UpdateManyAsync(objections, true);


        //    //var summary = await _repository.GetListAsync();
        //    //foreach (var item in summary)
        //    //{
        //    //    item.OfficeCode = offices.FirstOrDefault(o => o.id == item.OfficeId).code;
        //    //}
        //    //await _repository.UpdateManyAsync(summary, true);
        //    ////yearlyobjection

        //    //var yearlyObjections = await _yearlyObjection.GetListAsync();
        //    //foreach (var item in yearlyObjections)
        //    //{
        //    //    item.OfficeCode = offices.FirstOrDefault(o => o.id == item.OfficeId).code;
        //    //}
        //    //await _yearlyObjection.UpdateManyAsync(yearlyObjections, true);

        //    var officeUsers = await _officeUserRepo.GetListAsync();
        //    foreach (var item in officeUsers)
        //    {
        //        item.OfficeCode = offices.FirstOrDefault(o => o.id == item.OfficeId).code;
        //    }
        //    await _officeUserRepo.UpdateManyAsync(officeUsers, true);
        //}
    }
}
