﻿using System;
using Volo.Abp.Application.Dtos;

namespace PWD.Audit.DtoModels
{
    public class SummaryLineDto : EntityDto<int>
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
        public double Value { get; set; }
        public int BroadSheet { get; set; }
        public int NonBroadSheet { get; set; }
        public int Resolved { get; set; }
        public int Note { get; set; }
    }

}