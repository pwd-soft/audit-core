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
        public int Count { get; set; } = 0;
        public double Value { get; set; } = 0;
        public int BroadSheet { get; set; } = 0;
        public int NonBroadSheet { get; set; } = 0;
        public int Resolved { get; set; } = 0;
        public string Note { get; set; }
    }

}
