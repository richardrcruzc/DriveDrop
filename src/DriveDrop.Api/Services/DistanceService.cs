using DriveDrop.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public class DistanceService: IDistanceService
    {

        private readonly DriveDropContext _context;
        private readonly IHostingEnvironment _env;

        private readonly string _uploads;
        private readonly string _filePath;


        public DistanceService(IHostingEnvironment env, DriveDropContext context)
        {
            _context = context;
            _env = env;

             _uploads = Path.Combine(_env.WebRootPath, "uploads\\SeedData");
            _filePath = string.Format("{0}/2015_Gaz_zcta_national.txt", _uploads);


        }

        public async Task<Double> FromZipToZipInMile(int from, int to)
        {
            Double returnMile;

            using (StreamReader reader = File.OpenText(_filePath))
            {
                ZipCodes codes =await ZipCodeReader.ReadZipCodes(reader);
                codes.IsCaching = true;

                returnMile =  codes[from].Distance(codes[to], Measurement.Miles);



                //Console.WriteLine("From 90210 to 73487 in miles: {0:0.##}",
                //    codes[90210].Distance(codes[73487], Measurement.Miles));
                //Console.WriteLine("From 90210 to 73487 in kilometers: {0:0.##}",
                //    codes[90210].Distance(codes[73487], Measurement.Kilometers));
                //Console.WriteLine("From 13126 to 49728 in miles: {0:0.##}",
                //    codes[13126] - codes[49728]);

                //Console.WriteLine("Find all zips < 25 miles from 13126:");
                //var distanced = codes.FindLessThanDistance(codes[13126], 25);

                //if (distanced.Count() > 0)
                //{
                //    foreach (ZipCodeDistance code in codes.FindLessThanDistance(codes[13126], 25))
                //    {
                //        Console.WriteLine("* {0} ({1:0.##} miles)", code.ZipCode.Code, code.Distance);
                //    }
                //}

                //Console.WriteLine("Press any key to exit");
                //Console.ReadKey();
            }

             
            return returnMile;

        }
public async Task<Double> FromZipToZipInKilometer(int from, int to)
        {
            Double returnKilometers;
            using (StreamReader reader = File.OpenText(_filePath))
            {
                ZipCodes codes = await ZipCodeReader.ReadZipCodes(reader);
                codes.IsCaching = true;

                returnKilometers = codes[from].Distance(codes[to], Measurement.Kilometers);
            }

            return returnKilometers;
        }
        public async Task<IEnumerable<ZipCodeDistance>> FindLessThanDistance( int from, int mile) {

            IEnumerable<ZipCodeDistance> distanced = new  List<ZipCodeDistance>();

            using (StreamReader reader = File.OpenText(_filePath))
            {
                ZipCodes codes = await ZipCodeReader.ReadZipCodes(reader);
                codes.IsCaching = true;

                  distanced = codes.FindLessThanDistance(codes[from], mile);
            }

            return distanced;
        }
    }
}
