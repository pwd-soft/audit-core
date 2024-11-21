using PWD.Attendance_Swagger.DtoModels;
using PWD.Audit.DtoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit.Interfaces
{
    public interface IReportAppService
    {
        Task<GenericListDto<ObjectionReportDto>> DetailedReport(ReportFilterModel reportFilter);
    }
}
