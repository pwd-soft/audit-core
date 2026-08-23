using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class SummaryDto : EntityDto<int>
    {
        public string OfficeCode { get; set; } = string.Empty;
        public string OfficeName { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string ReferenceNo { get; set; }
        public int Note { get; set; }
        public List<SummaryLineDto> SummaryLines { get; set; }=new List<SummaryLineDto>();
        public string Layer { get; set; }
    }

}
