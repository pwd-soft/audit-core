using PWD.Audit.DtoModels;
using PWD.Audit.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class FilterModel
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public bool IsDesc { get; set; }
    }

    public class ObjectionFilterModel : FilterModel 
    {
        public string? OfficeCode { get; set; }
        public string? FinancialYear { get; set; }
        public ObjectionType? ObjectionType { get; set; }
        public DirectorateType? DirectorateType { get; set; }
        public ObjectionStatus? ObjectionStatus { get; set; }
        //public bool IsActive { get; set; } = true;
        //public DateTime Date { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public string Response { get; set; }
        //public double Value { get; set; }
        //public string Note { get; set; }
        //public string Attachments { get; set; }
        //public List<AssociateDto> Associates { get; set; }
    }
}
