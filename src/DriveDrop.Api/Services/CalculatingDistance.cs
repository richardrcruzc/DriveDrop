﻿//https://www.wiredprairie.us/blog/index.php/archives/688
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Services
{
    public enum Measurement
    {
        Miles,
        Kilometers
    }

    public class ZipCode
    {
        private double _cosLatitude = 0.0;
        private double _latitutde;
        private IEnumerable<ZipCodeDistance> _cachedZipDistance;
        /// <summary>
        /// Two-digit state code
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 5 digit Postal Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Latitude, in Radians
        /// </summary>
        public double Latitude
        {
            get { return _latitutde; }
            set
            {
                _latitutde = value;
                _cosLatitude = Math.Cos(value);
            }
        }

        /// <summary>
        /// Precomputed value of the Cosine of Latitutde
        /// </summary>
        private double CosineOfLatitutde
        {
            get { return _cosLatitude; }
        }
        /// <summary>
        /// Longitude, in Radians
        /// </summary>
        public double Longitude { get; set; }

        public double Distance(ZipCode compare)
        {
            return Distance(compare, Measurement.Miles);
        }

        /// <summary>
        /// Computes the distance between two zip codes using the Haversine formula
        /// (http://en.wikipedia.org/wiki/Haversine_formula).
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public double Distance(ZipCode compare, Measurement m)
        {
            double dLon = compare.Longitude - this.Longitude;
            double dLat = compare.Latitude - this.Latitude;

            double a = Math.Pow(Math.Sin(dLat / 2.0), 2) +
                    this.CosineOfLatitutde *
                    compare.CosineOfLatitutde *
                    Math.Pow(Math.Sin(dLon / 2.0), 2.0);

            double c = 2 * Math.Asin(Math.Min(1.0, Math.Sqrt(a)));
            double d = (m == Measurement.Miles ? 3956 : 6367) * c;

            return d;
        }

        public static double operator -(ZipCode z1, ZipCode z2)
        {
            if (z1 == null || z2 == null) { throw new ArgumentNullException(); }
            return z1.Distance(z2);
        }

        public static double ToRadians(double d)
        {
            return (d / 180) * Math.PI;
        }

        internal IEnumerable<ZipCodeDistance> DistanceCache
        {
            get
            {
                return _cachedZipDistance;
            }
            set
            {
                _cachedZipDistance = value;
            }
        }
    }

    public class ZipCodeDistance
    {
        public ZipCode ZipCode { get; set; }
        public double Distance { get; set; }
    }

    public class ZipCodes : Dictionary<int, ZipCode>
    {
        private const double MaxDistance = 100.0;

        /// <summary>
        /// Gets and sets whether the ZipCodes class caches all search results.
        /// </summary>
        public bool IsCaching { get; set; }
        /// <summary>
        /// Find all Zip Codes less than a specified distance
        /// </summary>
        /// <param name="startingZipCode">Provide the starting zip code as an object</param>
        /// <param name="distance">Maximum distance from starting zip code</param>
        /// <returns>List of ZipCodeDistance objects, sorted by distance.</returns>
        public IEnumerable<ZipCodeDistance> FindLessThanDistance(ZipCode startingZipCode, double distance)
        {
            if (distance > MaxDistance)
            {
                throw new ArgumentOutOfRangeException("distance",
                    string.Format("Must be less than {0}.", MaxDistance));
            }

            IEnumerable<ZipCodeDistance> codes1 = null;
            if (startingZipCode.DistanceCache == null)
            {
                // grab all less than the MaxDistance in first pass
                codes1 = from c in this.Values
                         let d = c - startingZipCode
                         where (d <= MaxDistance)
                         orderby d
                         select new ZipCodeDistance() { ZipCode = c, Distance = d };
                // this might just be temporary storage depending on caching settings
                startingZipCode.DistanceCache = codes1;
            }
            else
            {
                // grab the cached copy
                codes1 = startingZipCode.DistanceCache;
            }
            List<ZipCodeDistance> filtered = new List<ZipCodeDistance>();

            foreach (ZipCodeDistance zcd in codes1)
            {
                // since the list is pre-sorted, we can now drop out 
                // quickly and efficiently as soon as something doesn't
                // match
                if (zcd.Distance > distance)
                {
                    break;
                }
                filtered.Add(zcd);
            }

            // if no caching, don't leave the cached result in place
            if (!IsCaching) { startingZipCode.DistanceCache = null; }
            return filtered;
        }
    }

    public static class ZipCodeReader
    {
        public static async Task<ZipCodes> ReadZipCodes(Stream stream)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }

            using (StreamReader reader = new StreamReader(stream))
            {
                return await ReadZipCodes(reader);
            }
        }

        public static async Task<ZipCodes> ReadZipCodes(StreamReader reader)
        {
            /*
             * From documentation found here: http://www.census.gov/geo/www/gazetteer/places2k.html
                The ZCTA file contains data for all 5 digit ZCTAs in the 50 states, 
                         District of Columbia and Puerto Rico as of Census 2000. The file is plain ASCII text, one line per record.

                Columns 1-2: United States Postal Service State Abbreviation
                Columns 3-66: Name (e.g. 35004 5-Digit ZCTA - there are no post office names)
                Columns 67-75: Total Population (2000)
                Columns 76-84: Total Housing Units (2000)
                Columns 85-98: Land Area (square meters) - Created for statistical purposes only.
                Columns 99-112: Water Area (square meters) - Created for statistical purposes only.
                Columns 113-124: Land Area (square miles) - Created for statistical purposes only.
                Columns 125-136: Water Area (square miles) - Created for statistical purposes only.
                Columns 137-146: Latitude (decimal degrees) First character is blank or "-" denoting North or South latitude respectively
                Columns 147-157: Longitude (decimal degrees) First character is blank or "-" denoting East or West longitude respectively             
             */
            if (reader == null) { throw new ArgumentNullException("reader"); }

            ZipCodes codes = new ZipCodes();
            //string line = reader.ReadLine();
            string line = await reader.ReadLineAsync();
            while (!string.IsNullOrEmpty(line))
            {

                string[] bits = line.Split('\t');

                int code = 0;
                double lat = 0;
                double lon = 0;
                // skip lines that aren't valid
                //if (Int32.TryParse(line.Substring(2, 5), out code) &&
                //    double.TryParse(line.Substring(136, 10), out lat) &&
                //    double.TryParse(line.Substring(146, 10), out lon)
                //    )
                    if (Int32.TryParse(bits[0], out code) &&
                    double.TryParse(bits[5], out lat) &&
                    double.TryParse(bits[6], out lon)
                    )
                    {
                    // there are a few duplicates due to state boundaries,
                    // ignore them
                    if (!codes.ContainsKey(code))
                    {
                        codes.Add(code, new ZipCode()
                        {
                            State = line.Substring(0, 2),
                            Code = code,
                            Latitude = ZipCode.ToRadians(lat),
                            Longitude = ZipCode.ToRadians(lon),
                        });
                    }
                }
                line = reader.ReadLine();
            }
            return codes;
        }
    }
}
