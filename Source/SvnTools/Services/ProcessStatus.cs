using System;
using System.Collections.Generic;
using System.Text;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// Provides enumerated values that indicate the current status of a process.
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>Process is ready to run.</summary>
        Ready,
        /// <summary>Process is currently running.</summary>
        Running,
        /// <summary>Process is complete.</summary>
        Complete,
        /// <summary>Process ran with an error.</summary>
        Error,
        /// <summary>Process timed out.</summary>
        TimedOut
    }
}