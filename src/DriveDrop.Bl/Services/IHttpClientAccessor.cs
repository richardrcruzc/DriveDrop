using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface IHttpClientAccessor
    {
        HttpClient HttpClient { get;  }
    }
}
