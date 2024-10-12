using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class EmailSettings
    {
        public bool EmailNotification { get; set; }
        public bool SendCopyToPersonalEmail { get; set; }
        public ActivityRelatesEmail ActivityRelatesEmail { get; set; }
        public UpdatesFromKeenthemes UpdatesFromKeenthemes { get; set; }
    }
}
