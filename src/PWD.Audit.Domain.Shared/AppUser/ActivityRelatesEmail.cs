using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit
{
    public class ActivityRelatesEmail
    {
        public bool YouHaveNewNotifications { get; set; }
        public bool YouAreSentADirectMessage { get; set; }
        public bool SomeoneAddsYouAsAsAConnection { get; set; }
        public bool UponNewOrder { get; set; }
        public bool NewMembershipApproval { get; set; }
        public bool MemberRegistration { get; set; }
    }
}
