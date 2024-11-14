using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class ReportFilterModel
    {
        public ReportType ReportType { get; set; } = ReportType.All;
        public DirectorateType DirectorateType { get; set; } = DirectorateType.None;
        public ObjectionType ObjectionType { get; set; } = ObjectionType.None;
        public List<Guid> Offices { get; set; }
    }
}
