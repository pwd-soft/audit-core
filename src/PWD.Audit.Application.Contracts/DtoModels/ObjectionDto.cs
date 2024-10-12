using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace PWD.Attendance_Swagger.DtoModels
{
    public class ObjectionDto  : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Response { get; set; }
        public double Value { get; set; }
        public bool IsBroadSheet { get; set; }
        public bool IsResolved { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public List<AssociateDto> Associates { get; set; }
    }

}
