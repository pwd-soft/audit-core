using System;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class AssociateDto : FullAuditedEntityDto<int>
    {
        public int ObjectionId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Post { get; set; }
        public string Note { get; set; }
    }

}
