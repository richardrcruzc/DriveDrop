using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
    }
}
