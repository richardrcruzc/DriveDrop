using System.Threading.Tasks;
using DriveDrop.Core.Models.Browser;

namespace DriveDrop.Core.Services.Dependency
{
    public interface INativeBrowserService
    {
        Task<BrowserResult> LaunchBrowserAsync(string url);
    }
}
