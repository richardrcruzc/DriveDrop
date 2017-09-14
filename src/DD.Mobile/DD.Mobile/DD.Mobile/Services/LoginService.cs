using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DD.Mobile.Models;

namespace DD.Mobile.Services
{
  public  class LoginService:ILoginService
    {
        public void Dispose()
        {
        }

        public Task LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => {
                //Just for testing
                //HACK: 
                //Thread.Sleep (2000);

                //for (int i = 0;i<1000 ; i++)
                //{
                //    var yy = i;

                //}
            });
        }

        public Task<IList<User>> SearchAsync(Filter filter, int sizeLimit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
