using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public interface IPayPalStandardPaymentProcessor
    {
        Task PostProcessPayment(int shipmentId);
    }
}
