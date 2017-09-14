using DD.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DD.Mobile.Services
{ 
    public interface ILoginService : IDisposable
    {
        Task LoginAsync(string username, string password, CancellationToken cancellationToken);

        Task<IList<User>> SearchAsync(Filter filter, int sizeLimit, CancellationToken cancellationToken);
    }
}
