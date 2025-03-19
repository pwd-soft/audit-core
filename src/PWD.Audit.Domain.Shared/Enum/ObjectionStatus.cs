using System;
using System.Collections.Generic;
using System.Text;

namespace PWD.Audit.Enum
{
    public enum ObjectionStatus
    {
        None,
        BroadSheetNotAnswered,
        BroadSheetAnswered,
        RequestedReAnswer,
        ReAnswered,
        Resolved,
        Other,
        RequestedReAnswerFromMinistry,
        RequestedReAnswerFromAGOffice
    }
}
