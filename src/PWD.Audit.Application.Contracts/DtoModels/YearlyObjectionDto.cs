using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace PWD.Audit.DtoModels
{
    public class YearlyObjectionDto : Entity<int>
    {
        public Guid OfficeId { get; set; }
        public int Year { get; set; }
        public int NumberOfObjections { get; set; }
    }
}
