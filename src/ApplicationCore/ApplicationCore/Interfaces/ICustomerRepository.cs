using ApplicationCore.Entities.Helpers;
using ApplicationCore.SeedWork; 
using System.Threading.Tasks;

namespace ApplicationCore.Entities.ClientAgregate
{
   
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Buyer Aggregate

    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomers(int pageIndex, int itemsPage, int? type, int? status, int? transport, string lastname);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<Customer> FindAsync(string UserIdentityGuid);

        Task<Customer> GetCustomerByUser(string user);
        Task<Customer> GetAsync(int orderId);

    }
     
}
