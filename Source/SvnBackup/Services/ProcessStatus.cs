using System;
using System.Collections.Generic;
using System.Text;

// $Id$

namespace SvnBackup.Services
{
    public enum ProcessStatus
    {
        Ready,
        Running,
        Complete,
        Error,
        TimedOut
    }
}