using PWD.Audit.Enum;
using System;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class SummaryLineDto : EntityDto<int>
    {
        public int SummaryId { get; set; }
        public ObjectionType Type { get; set; }
        public string TypeName { get; set; }
        public int Count { get; set; }
        public double Value { get; set; }
        public int BroadSheet { get; set; }
        public int NonBroadSheet { get; set; }
        public int Resolved { get; set; } 
        public string Note { get; set; }
    }

}
