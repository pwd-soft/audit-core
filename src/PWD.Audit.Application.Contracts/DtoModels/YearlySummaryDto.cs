using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class YearlySummaryDto
    {
        public string OfficeCode { get; set; } = string.Empty;
        public string OfficeName { get; set; }
        public List<YearlySummaryDetailsDto> YearlySummaryDetails { get; set; } = new List<YearlySummaryDetailsDto>();
    }
    public class YearlySummaryDetailsDto
    {
        public string FinancialYear { get; set; }
        public List<SummaryLineDto> SummaryLines { get; set; } = new List<SummaryLineDto>();
    }
}
