using System;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class SummaryDto : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string ReferenceNo { get; set; }
        public int Note { get; set; }
    }

}
