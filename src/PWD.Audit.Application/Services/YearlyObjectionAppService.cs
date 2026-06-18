using Microsoft.AspNetCore.Authorization;
using PWD.Audit.DtoModels;
using PWD.Audit.Interfaces;
using PWD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace PWD.Audit.Services
{
    [Authorize]
    public class YearlyObjectionAppService : ApplicationService, IYearlyObjectionAppService
    {
        private readonly IRepository<YearlyObjection, int> _repository;
        private IApprovalAppService _approvalAppService;

        public YearlyObjectionAppService(IRepository<YearlyObjection, int> repository, IApprovalAppService approvalAppService)
        {
            _repository = repository;
            _approvalAppService = approvalAppService;
        }

        public async Task<YearlyObjectionDto> GetByIdAsync(int id)
        {
            var yearlyObjection = await _repository.GetAsync(id);
            return ObjectMapper.Map<YearlyObjection, YearlyObjectionDto>(yearlyObjection);
        }

        public async Task<List<YearlyObjectionDto>> GetListByOfficeCodeAsync(string officeCode)
        {
            var listOfYearlyObjections = await _repository.GetListAsync(y => y.OfficeCode == officeCode);
            return ObjectMapper.Map<List<YearlyObjection>, List<YearlyObjectionDto>>(listOfYearlyObjections);
        }

        public async Task<YearlyObjectionDto> UpdateAsync(YearlyObjectionDto input)
        {
            var dbItem = await _repository.GetAsync(input.Id);
            
            if (dbItem is not null)
            {
                dbItem.NumberOfObjections = input.NumberOfObjections;
            }
            
            var updatedItem = await _repository.UpdateAsync(dbItem);

            return ObjectMapper.Map<YearlyObjection, YearlyObjectionDto>(updatedItem);
        }

        //public async Task PopulateYearlyObjections()
        //{
        //    const string AuditMonitoringOfficeRole = "AuditOfficeAdmin";

        //    var offices = await _approvalAppService.GetOffices();
        //    var auditees = await _approvalAppService.GetUserByRole(AuditMonitoringOfficeRole);

        //    offices = offices.Where(x => auditees.Contains(x.code)).ToList();

        //    List<YearlyObjection> listOfYearlyObjections = new List<YearlyObjection>();
        //    foreach (var office in offices)
        //    {
        //        for (int i = 1972; i < 2025; i++)
        //        {
        //            var yearlyObjection = new YearlyObjection()
        //            {
        //                OfficeId = (Guid)office.id,
        //                Year = i,
        //                NumberOfObjections = 0
        //            };
        //            listOfYearlyObjections.Add(yearlyObjection);
        //        }
        //    }

        //    await _yearlyObjectionRepository.InsertManyAsync(listOfYearlyObjections);
        //}


    }
}
