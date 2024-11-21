using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class ObjectionReportDto
    {
        public OrganizationUnitDto OrgUnit { get; set; }
        public List<ObjectionDto> Objections { get; set; }
    }
}
