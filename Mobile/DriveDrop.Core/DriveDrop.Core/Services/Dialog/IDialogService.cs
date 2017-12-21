using System.Threading.Tasks;

namespace DriveDrop.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
    }
}
