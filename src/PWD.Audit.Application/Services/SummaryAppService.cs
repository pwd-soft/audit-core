using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
                summary.Layer = office.layer;
                result.Add(summary);
            }
            return result;
        }

        public async Task<SummaryDto> GetByOffice(string officeCode)
        {
            var objectionQuery = await _objectionRepository.GetQueryableAsync();
            var objections = objectionQuery.Where(x => x.OfficeCode == officeCode);
            var sfi = objections.Where(o => o.ObjectionType == ObjectionType.SFI);
            var nsfi = objections.Where(o => o.ObjectionType == ObjectionType.NonSFI);
            var dr = objections.Where(o => o.ObjectionType == ObjectionType.Draft);
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

        public async Task<YearlySummaryDto> GetYearlySummary(string officeCode)
        {
            var objectionQuery = await _objectionRepository.GetQueryableAsync();
            var objections = objectionQuery.Where(x => x.OfficeCode == officeCode);
            var finacialYears = objections.Select(x => x.FinancialYear).Distinct().ToList();

            var result = new YearlySummaryDto() { OfficeCode = officeCode };
            //result.TotalSummary
            //s.typeName
            //s.count
            //s.value
            //s.broadSheet
            //s.nonBroadSheet
            //s.resolved

            foreach (var year in finacialYears)
            {
                result.YearlySummaryDetails ??= new List<YearlySummaryDetailsDto>();
                YearlySummaryDetailsDto yearlySummaryDetail = new YearlySummaryDetailsDto();
                yearlySummaryDetail.FinancialYear = year;

                var yearlyObjections = objections.Where(x => x.FinancialYear == year);
                var sfi = yearlyObjections.Where(o => o.ObjectionType == ObjectionType.SFI);
                var nsfi = yearlyObjections.Where(o => o.ObjectionType == ObjectionType.NonSFI);
                var dr = yearlyObjections.Where(o => o.ObjectionType == ObjectionType.Draft);

                // Process the yearly objections and create YearlySummaryDetailsDto
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
                    Count = sfiLine.Count + nsfiLine.Count + drLine.Count,
                    BroadSheet = sfiLine.BroadSheet + nsfiLine.BroadSheet + drLine.BroadSheet,
                    Resolved = sfiLine.Resolved + nsfiLine.Resolved + drLine.Resolved,
                    NonBroadSheet = sfiLine.NonBroadSheet + nsfiLine.NonBroadSheet + drLine.NonBroadSheet,
                    Value = sfiLine.Value + nsfiLine.Value + drLine.Value,
                    Type = Enum.ObjectionType.None,
                    TypeName = "উপমোট",
                };
                yearlySummaryDetail.SummaryLines.Add(sfiLine);
                yearlySummaryDetail.SummaryLines.Add(nsfiLine);
                yearlySummaryDetail.SummaryLines.Add(drLine);
                yearlySummaryDetail.SummaryLines.Add(totalLine);

                result.TotalSummary.Count += totalLine.Count;
                result.TotalSummary.BroadSheet += totalLine.BroadSheet;
                result.TotalSummary.Resolved += totalLine.Resolved;
                result.TotalSummary.NonBroadSheet += totalLine.NonBroadSheet;
                result.TotalSummary.Value += totalLine.Value;

                result.YearlySummaryDetails.Add(yearlySummaryDetail);
            }

            return result;
        }

        public async Task<List<SummaryDto>> GetListAsync() => ObjectMapper.Map<List<Summary>, List<SummaryDto>>(await _repository.GetListAsync());

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
        //2018-07
        public async Task<List<SummaryReportDto>> GenerateSummaryReport(SummaryReportInputDto summaryReportCriteria)
        {
            List<SummaryReportDto> SummaryReportData = new List<SummaryReportDto>();
            offices = await _approvalAppService.GetOffices();

            switch (summaryReportCriteria.Type)
            {
                case SummaryReportType.PWD:
                case SummaryReportType.Combined:
                case SummaryReportType.Detailed:
                    if (summaryReportCriteria.Offices.Count > 0)
                    {
                        SummaryReportData = await ProcessSummaryData(summaryReportCriteria.Offices, summaryReportCriteria.Type, summaryReportCriteria.SubType, summaryReportCriteria.Month);
                    }
                    break;
                case SummaryReportType.Officewise:
                    SummaryReportData = await GetOfficeData(summaryReportCriteria.Offices, summaryReportCriteria.Month);
                    break;
            }


            int serial = 1;
            foreach (var item in SummaryReportData)
            {
                item.Serial = serial;
                serial++;
            }

            return SummaryReportData;
        }

        private async Task<List<SummaryReportDto>> ProcessSummaryData(List<string> OfficeIds, SummaryReportType type, SummaryReportSubType subType, string month)
        {
            List<SummaryReportDto> Data = new List<SummaryReportDto>();

            foreach (var id in OfficeIds)
            {
                var result = new List<SummaryReportDto>();

                switch (subType)
                {
                    case SummaryReportSubType.Zonewise:
                        result = await GetZoneData(id, type, month);
                        break;
                    case SummaryReportSubType.Circlewise:
                        result = await GetCircleData(id, type, month);
                        break;
                }

                Data.AddRange(result);
            }

            return Data;
        }

        private async Task<List<SummaryReportDto>> GetZoneData(string officeCode, SummaryReportType type, string month)
        {
            List<SummaryReportDto> zoneSummaryList = new List<SummaryReportDto>();
            var objectionList = await _objectionAppService.GetListByOfficeCodeAsync(officeCode);
            var summary = AssignData(objectionList, month);
            var office = offices.FirstOrDefault(o => o.code == officeCode);
            summary.Name = office?.displayNameBn;
            summary.Layer = office?.layer;
            zoneSummaryList.Add(summary);

            var circles = offices.Where(o => o.parentCode == officeCode).ToList();

            var summaryList = new List<SummaryReportDto>();

            foreach (var item in circles)
            {
                var circleSummary = await GetCircleData(item.code, type, month);

                if (type == SummaryReportType.Combined)
                {
                    zoneSummaryList = CombineData(zoneSummaryList, circleSummary);
                }

                if (type == SummaryReportType.Detailed)
                {
                    zoneSummaryList.AddRange(circleSummary);
                }
            }

            return zoneSummaryList;
        }

        private async Task<List<SummaryReportDto>> GetCircleData(string officeCode, SummaryReportType type, string month)
        {
            List<SummaryReportDto> circleSummaryList = new List<SummaryReportDto>();

            var circleObjectionList = await _objectionAppService.GetListByOfficeCodeAsync(officeCode);
            var circleSummary = AssignData(circleObjectionList, month);
            var office = offices.FirstOrDefault(o => o.code == officeCode);
            circleSummary.Name = office?.displayNameBn;
            circleSummary.Layer = office?.layer;
            circleSummaryList.Add(circleSummary);

            var divisions = offices.Where(o => o.parentCode == officeCode).ToList();

            foreach (var item in divisions)
            {
                var divisionSummary = await GetDivisionData(item.code, type, month);

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

        private async Task<List<SummaryReportDto>> GetDivisionData(string officeCode, SummaryReportType type, string month)
        {
            List<SummaryReportDto> divisionSummaryList = new List<SummaryReportDto>();
            var divisionInfo = offices.FirstOrDefault(o => o.code == officeCode);
            var objectionList = await _objectionAppService.GetListByOfficeCodeAsync(officeCode);
            var summary = AssignData(objectionList, month);
            summary.Name = divisionInfo?.displayNameBn;
            summary.Layer = divisionInfo?.layer;

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

            return divisionSummaryList;
        }

        private async Task<List<SummaryReportDto>> GetOfficeData(List<string> OfficeIds, string month)
        {
            var officeSummaryList = new List<SummaryReportDto>();
            foreach (var item in OfficeIds)
            {
                var objections = await _objectionAppService.GetListByOfficeCodeAsync(item);
                var summary = AssignData(objections, month);

                var office = offices.FirstOrDefault(o => o.code == item);
                summary.Name = office?.displayNameBn;
                summary.Layer = office?.layer;
                officeSummaryList.Add(summary);
            }

            return officeSummaryList;
        }

        private SummaryReportDto AssignData(List<ObjectionDto> list, string month)
        {
            //var currentDay = DateTime.Today;
            //var firstDayOfCurrentMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
            //var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
            //var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
            var numbers = Array.ConvertAll(month.Split('-'), int.Parse).ToList();
            //var currentDay = DateTime.Today;
            var firstDayOfCurrentMonth = new DateTime(numbers[0], numbers[1], 1);
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

            //summary.CurrentObjectionSettlementNumber = currentMonthData.Count(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay);
            //summary.CurrentObjectionSettlementAmount = currentMonthData.Where(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay).Sum(s => s.Value);
            summary.CurrentObjectionSettlementNumber = currentMonthData.Count(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= firstDayOfCurrentMonth);
            summary.CurrentObjectionSettlementAmount = currentMonthData.Where(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= firstDayOfCurrentMonth).Sum(s => s.Value);

            summary.NonSfiNumber = list.Count(l => l.ObjectionType == ObjectionType.NonSFI) - list.Count(l => l.ObjectionType == ObjectionType.NonSFI && l.MemoNumber is not null);
            summary.SfiNumber = list.Count(l => l.ObjectionType == ObjectionType.SFI) - list.Count(l => l.ObjectionType == ObjectionType.SFI && l.MemoNumber is not null);
            summary.DraftNumber = list.Count(l => l.ObjectionType == ObjectionType.Draft) - list.Count(l => l.ObjectionType == ObjectionType.Draft && l.MemoNumber is not null);
            summary.TotalObjectionNumber = summary.NonSfiNumber + summary.SfiNumber + summary.DraftNumber;
            summary.TotalObjectionAmount = list.Where(l => l.MemoNumber == null).Sum(s => s.Value);

            return summary;
        }

        //private SummaryReportDto AssignData(List<ObjectionDto> list)
        //{
        //    var currentDay = DateTime.Today;
        //    var firstDayOfCurrentMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
        //    var firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
        //    var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);

        //    var previousMonthData = list.Where(l => l.ObjectionDate >= firstDayOfPreviousMonth && l.ObjectionDate <= lastDayOfPreviousMonth).ToList();
        //    var currentMonthData = list.Where(l => l.ObjectionDate >= firstDayOfCurrentMonth).ToList();

        //    var summary = new SummaryReportDto();

        //    summary.PreviousObjectionNumber = previousMonthData.Count();
        //    summary.PreviousObjectionAmount = previousMonthData.Sum(p => p.Value);

        //    summary.CurrentObjectionNumber = currentMonthData.Count();
        //    summary.CurrentObjectionAmount = currentMonthData.Sum(p => p.Value);

        //    summary.SubTotalObjectionNumber = summary.PreviousObjectionNumber + summary.CurrentObjectionNumber;
        //    summary.SubTotalObjectionAmount = summary.PreviousObjectionAmount + summary.CurrentObjectionAmount;

        //    summary.PreviousBroadsheetNumber = previousMonthData.Count(p => p.ObjectionStatus == ObjectionStatus.BroadSheetAnswered);
        //    summary.UnsetteledBroadsheetNumber = list.Count - list.Count(l => l.ObjectionStatus == ObjectionStatus.Resolved) - list.Count(l => l.ObjectionStatus == ObjectionStatus.BroadSheetNotAnswered);

        //    summary.CurrentObjectionSettlementNumber = currentMonthData.Count(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay);
        //    summary.CurrentObjectionSettlementAmount = currentMonthData.Where(c => c.MemoDate >= firstDayOfCurrentMonth && c.MemoDate <= currentDay).Sum(s => s.Value);

        //    summary.NonSfiNumber = list.Count(l => l.ObjectionType == ObjectionType.NonSFI) - list.Count(l => l.ObjectionType == ObjectionType.NonSFI && l.MemoNumber is not null);
        //    summary.SfiNumber = list.Count(l => l.ObjectionType == ObjectionType.SFI) - list.Count(l => l.ObjectionType == ObjectionType.SFI && l.MemoNumber is not null);
        //    summary.DraftNumber = list.Count(l => l.ObjectionType == ObjectionType.Draft) - list.Count(l => l.ObjectionType == ObjectionType.Draft && l.MemoNumber is not null);
        //    summary.TotalObjectionNumber = summary.NonSfiNumber + summary.SfiNumber + summary.DraftNumber;
        //    summary.TotalObjectionAmount = list.Where(l => l.MemoNumber == null).Sum(s => s.Value);

        //    return summary;
        //}

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

        //public Task<bool> Test(string month)
        //{
        //    var numbers = Array.ConvertAll(month.Split('-'), int.Parse).ToList();
        //    return Task.FromResult(true);
        //}
    }
}
