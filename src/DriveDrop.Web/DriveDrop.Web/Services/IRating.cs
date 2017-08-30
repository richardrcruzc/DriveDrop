using DriveDrop.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Web.Services
{ 
    public interface IRatingRepository
    {
        Task<ReviewModel> GetAsync(int shippingId);        
        Task<ReviewModel> UpdateAsync(ReviewModel rating);
        Task<bool> DeleteAsync(int id);
    }
}
