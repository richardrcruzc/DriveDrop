using ApplicationCore.Entities.Helpers;
using DriveDrop.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public interface IRateService
    {
        Task<bool> Add(Rate rate);    

        Task<bool> Update(Rate rate);
        Task<List<Rate>> Get();
        Task<Rate> Get(int id);

        // Task<decimal> Distance(string fromZip, string toZip);


        Task<CalculatedCharge> CalculateAmount(int zipFrom, int zipTo, decimal weight, int qty, int priority, int transportTypeId, string promoCode);

    }
}
