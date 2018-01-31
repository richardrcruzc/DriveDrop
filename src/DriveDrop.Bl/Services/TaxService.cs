using ApplicationCore.Entities.Helpers;
using DriveDrop.Bl.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class TaxService: ITaxService
    {
        private readonly DriveDropContext _context;
        public TaxService(DriveDropContext context)
        {
            _context = context;
        }
        public async Task<List<Tax>> GetTaxes()
        {

            var rate = await _context.TaxRates
                .OrderBy(x => x.State).ThenBy(x => x.County).ThenBy(x => x.City)
                .ToListAsync();


            return rate;
        }
        public async Task<Tax> GetTax(int id)
        {
            var tax = await _context.TaxRates
              .Where(x => x.Id == id).FirstOrDefaultAsync();


            return tax;

        }
        public async Task<Tax> SaveTax(Tax m)
        {
           if(m == null)
                return null;

            if (m.RateDefault == true)
            {
                var taxes = await _context.TaxRates.Where(x => x.RateDefault).FirstOrDefaultAsync();
                if (taxes != null)
                {
                    taxes.SetDefault(false);
                    _context.Update(taxes);

                    await _context.SaveChangesAsync();
                }
            }

            var tax = await _context.TaxRates.Where(x => x.Id == m.Id).FirstOrDefaultAsync();

            if (tax == null)
            {
                tax = new Tax(m.State, m.County, m.City, m.Rate, m.RateDefault);
                _context.Add(tax);
            }
            else
            {
                tax.Update(m.State, m.County, m.City, m.Rate, m.RateDefault);
                _context.Update(tax);
            }

            await _context.SaveChangesAsync();

            return tax;
        }
        public async Task<bool> DeleteTax(int id)
        {


            var tax = await _context.TaxRates
                .OrderBy(x => x.Id)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (tax == null)
                return false;

            _context.Remove(tax);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
