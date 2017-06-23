using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Execeptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class DriveDropException : Exception
    {
        public DriveDropException()
        { }

        public DriveDropException(string message)
            : base(message)
        { }

        public DriveDropException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
