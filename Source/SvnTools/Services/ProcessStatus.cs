using System;
using System.Collections.Generic;
using System.Text;

// $Id$

namespace SvnTools.Services
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