using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class MonitoringAppService : ApplicationService, IMonitoringAppService
    {
        //private readonly IRepository<Objection, int> _objectionRepository;
        private readonly IObjectionAppService _objectionService;

//IRepository<Objection, int> objectionRepository,
        public MonitoringAppService( IObjectionAppService objectionService)
        {
            //_objectionRepository = objectionRepository;
            _objectionService = objectionService;
        }

        public async Task<int> GetResponseCountByOffice(string officeCode)
        {
            var latestResponses = await _objectionService.GetIncomingResponseListAsync(officeCode);
            return latestResponses.Count;
        }
    }


}
