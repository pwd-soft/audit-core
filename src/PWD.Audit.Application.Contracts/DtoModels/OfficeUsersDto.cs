using System;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class OfficeUserDto : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public int PostingId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Post { get; set; }
        public bool IsActive { get; set; }
    }

}
