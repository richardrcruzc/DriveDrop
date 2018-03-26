using GoDriveDrop.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoDriveDrop.Core.Services.Location
{
    public interface ILocationServiceImplementation
    {
        double DesiredAccuracy { get; set; }
        bool IsGeolocationAvailable { get; }
        bool IsGeolocationEnabled { get; }

        Task<Position> GetPositionAsync(TimeSpan? timeout = null, CancellationToken? token = null);
    }
}
