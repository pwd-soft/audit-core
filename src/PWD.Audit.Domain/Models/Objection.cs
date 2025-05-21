
using PWD.Audit.Enum;
using PWD.Audit.Models;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;


namespace PWD.Audit.Entities
{
    public class Objection:FullAuditedEntity<int>
    {
        public string OfficeCode { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public ObjectionType ObjectionType { get; set; }
        public DirectorateType DirectorateType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Response { get; set; }
        public double Value { get; set; }
        //public bool IsBroadSheet { get; set; } = false;
        //public bool IsResolved { get; set; } = false;
        public ObjectionStatus ObjectionStatus { get; set; } = ObjectionStatus.None;
        public bool IsActive { get; set; } = true;
        public string Note { get; set; }
        public string Attachments { get; set; }
        public List<Associate> Associates { get; set; }
        public string MemoNumber { get; set; }
        public DateTime MemoDate { get; set; }
        public string ObjectionMemoNumber { get; set; }
        public DateTime ObjectionDate { get; set; }
        public int AnswerCount { get; set; } = 0;
        public string Comments { get; set; } = string.Empty;
        public string ArticleNumber { get; set; } = string.Empty;
        public string CurrentOffice { get; set; } = string.Empty;
        public bool IsCentralEntry { get; set; } = false;


        public List<ResponseHistory> ResponseHistories { get; set; }
    }
}
