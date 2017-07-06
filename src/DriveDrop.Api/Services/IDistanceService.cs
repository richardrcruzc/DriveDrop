using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public interface IDistanceService
    {
        Task<Double> FromZipToZipInMile(int from, int to);
        Task<Double> FromZipToZipInKilometer(int from, int to);
        Task<IEnumerable<ZipCodeDistance>> FindLessThanDistance(int from, int mile);
    }
}
