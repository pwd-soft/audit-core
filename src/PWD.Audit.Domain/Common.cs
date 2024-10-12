using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWD.Audit
{
    public class Common
    {
        public Common()
        {
            IsActive = true;
            CreatedAt = DateTime.Now;
        }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
