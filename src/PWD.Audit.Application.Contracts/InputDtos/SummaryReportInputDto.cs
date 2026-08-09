using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.InputDtos
{
    public class SummaryReportInputDto
    {
        public SummaryReportType Type { get; set; }
        public SummaryReportSubType SubType { get; set; }
        public List<string> Offices { get; set; }
        public string Month { get; set; } = string.Empty;
    }
}
