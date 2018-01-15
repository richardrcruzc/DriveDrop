using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveDrop.Core.Services.Dependency
{
    public class DependencyService : IDependencyService
    {
        public T Get<T>() where T : class
        {
            return Xamarin.Forms.DependencyService.Get<T>();
        }
    }
}
