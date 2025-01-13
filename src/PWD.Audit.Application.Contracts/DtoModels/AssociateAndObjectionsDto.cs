using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class AssociateAndObjectionsDto
    {
        public AssociateDto Associate { get; set; }
        public List<ObjectionDto> Objections { get; set; }
    }
}
