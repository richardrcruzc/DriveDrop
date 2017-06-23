using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Execeptions
{
    
    public class ImageMissingException : Exception
    {
        public ImageMissingException(string message,
            Exception innerException = null)
            : base(message, innerException: innerException)
        {
        }
        public ImageMissingException(Exception innerException)
            : base("No image found for the provided id.",
                  innerException: innerException)
        {
        }
    }
}
