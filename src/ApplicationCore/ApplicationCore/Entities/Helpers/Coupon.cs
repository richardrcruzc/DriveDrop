using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities.Helpers
{
  public  class Coupon : Entity
    {
        public Coupon() { }
        public string Code { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool MultipleTime { get; private set; }
        public bool Percentage { get; private set; }
        public decimal Amount { get; private set; }

        public Coupon(string code, DateTime startDate, DateTime endDate, bool multipleTime, bool percentage, decimal amount) {
            Code = code;
            StartDate = startDate;
            EndDate = endDate;
            MultipleTime = multipleTime;
            Percentage = percentage;
            Amount = amount;
        }
    }
}
