using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class ObjectionDto  : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public ObjectionType ObjectionType { get; set; }
        public DirectorateType DirectorateType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Response { get; set; }
        public double Value { get; set; }
        public bool IsBroadSheet { get; set; }=false;
        public bool IsResolved { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Note { get; set; }
        public string Attachments { get; set; }
        public List<AssociateDto> Associates { get; set; }
    }

}
