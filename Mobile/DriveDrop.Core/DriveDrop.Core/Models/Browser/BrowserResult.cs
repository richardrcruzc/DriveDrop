using DriveDrop.Core.Extensions;

namespace DriveDrop.Core.Models.Browser
{
    public class BrowserResult
    {
        public BrowserResultType ResultType { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
        public bool IsError => Error.IsPresent();
    }
}
