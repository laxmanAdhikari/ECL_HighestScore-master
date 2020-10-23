using System;
using System.Collections.Generic;
using System.Text;

namespace ECL_HighestScore.Models
{
    /// <summary>
    /// Enum for Program exit codes
    /// </summary>
    public enum ProgramExitCode : int
    {
        Success =0,
        FileNotFound=1,
        InvalidData=2,
        InvalidInput=3,
        Fail=4
    }
}
