namespace GoDriveDrop.Core.Services.Location
{
    using System.Threading.Tasks;
    using GoDriveDrop.Core.Models.Commons;

    public interface ILocationService
    {
        Task UpdateUserLocation(Location newLocReq, string token);
    }
}