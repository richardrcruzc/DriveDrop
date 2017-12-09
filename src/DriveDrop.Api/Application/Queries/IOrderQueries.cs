namespace Microsoft.eShopOnContainers.Services.DriveDrop.Api.Application.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderQueries
    {
        Task<dynamic> GetOrderAsync(int id);

        Task<IEnumerable<dynamic>> GetOrdersAsync();

        Task<IEnumerable<dynamic>> GetCardTypesAsync();
    }
}
