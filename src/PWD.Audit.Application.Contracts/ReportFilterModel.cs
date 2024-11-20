using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class ReportFilterModel : FilterModel
    {
        //public ReportType ReportType { get; set; } = ReportType.All;
        public DirectorateType DirectorateType { get; set; } = DirectorateType.None;
        public ObjectionType ObjectionType { get; set; } = ObjectionType.None;
        public ObjectionStatus ObjectionStatus { get; set; } = ObjectionStatus.None;
        public List<Guid> Offices { get; set; }
        public string FinancialYear { get; set; } = string.Empty;
    }
}
