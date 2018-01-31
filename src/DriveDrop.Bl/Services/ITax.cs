using ApplicationCore.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface ITaxService
    {
        Task<List<Tax>> GetTaxes();
        Task<Tax> GetTax(int id);
        Task<Tax> SaveTax(Tax tax);
        Task<bool> DeleteTax(int id);
    }
}
