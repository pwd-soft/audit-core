using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace PWD.Audit.Models
{
    public class ResponseComment : FullAuditedEntity<int>
    {
        public int ResponseHistoryId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Office { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public int PostingId { get; set; } = 0;
    }
}
