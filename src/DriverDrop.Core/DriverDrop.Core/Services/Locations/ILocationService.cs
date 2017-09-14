namespace DriverDrop.Core.Services.Locations
{
    using System.Threading.Tasks;
    using Models.Locations;
    
    public interface ILocationService
    {
        Task UpdateUserLocation(Location newLocReq, string token);
    }
}