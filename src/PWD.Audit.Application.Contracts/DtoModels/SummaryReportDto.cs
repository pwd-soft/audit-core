using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.DtoModels
{
    public class SummaryReportDto
    {
        public int serial { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int previousObjectionNumber { get; set; } = 0;
        public double previousObjectionAmount { get; set; } = 0.0;
        public int currentObjectionNumber { get; set; } = 0;
        public double currentObjectionAmount { get; set; } = 0.0;
        public int subTotalObjectionNumber { get; set; } = 0;
        public double subTotalObjectionAmount { get; set; } = 0.0;
        public int previousBroadsheetNumber { get; set; } = 0;
        public int unsetteledBroadsheetNumber { get; set; } = 0;
        public int currentObjectionSettlementNumber { get; set; } = 0;
        public double currentObjectionSettlementAmount { get; set; } = 0.0;
        public int nonSfiNumber { get; set; } = 0;
        public int sfiNumber { get; set; } = 0;
        public int draftNumber { get; set; } = 0;
        public int totalObjectionNumber { get; set; } = 0;
        public double totalObjectionAmount { get; set; } = 0.0;
        public string comments { get; set; } = string.Empty;
    }
}
