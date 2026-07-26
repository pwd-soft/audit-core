using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class SummaryReportDto
    {
        public int Serial { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Layer { get; set; } = string.Empty;
        public int PreviousObjectionNumber { get; set; } = 0;
        public double PreviousObjectionAmount { get; set; } = 0.0;
        public int CurrentObjectionNumber { get; set; } = 0;
        public double CurrentObjectionAmount { get; set; } = 0.0;
        public int SubTotalObjectionNumber { get; set; } = 0;
        public double SubTotalObjectionAmount { get; set; } = 0.0;
        public int PreviousBroadsheetNumber { get; set; } = 0;
        public int UnsetteledBroadsheetNumber { get; set; } = 0;
        public int CurrentObjectionSettlementNumber { get; set; } = 0;
        public double CurrentObjectionSettlementAmount { get; set; } = 0.0;
        public int NonSfiNumber { get; set; } = 0;
        public int SfiNumber { get; set; } = 0;
        public int DraftNumber { get; set; } = 0;
        public int TotalObjectionNumber { get; set; } = 0;
        public double TotalObjectionAmount { get; set; } = 0.0;
        public string Comments { get; set; } = string.Empty;
    }
}
