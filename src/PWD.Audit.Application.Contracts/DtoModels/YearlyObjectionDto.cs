using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace PWD.Audit.DtoModels
{
    public class YearlyObjectionDto : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public string OfficeCode { get; set; } = string.Empty;
        public int Year { get; set; }
        public int NumberOfObjections { get; set; }
    }
}
