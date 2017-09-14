using ApplicationCore.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Helpers
{
    public class PackageSize
       : Enumeration
    { 
        public static PackageSize Envelopes = new PackageSize(1, "Envelope"); 
        public static PackageSize SmallPackages = new PackageSize(2, "Small Package");
        public static PackageSize MidiunPackages = new PackageSize(3, "Medium Package");
        public static PackageSize LargePackages = new PackageSize(4, "Large Package");
        public static PackageSize XLargePackages = new PackageSize(4, "Xtra Large Package");
        public static PackageSize OddPackages = new PackageSize(4, "Odd Size Package");

        protected PackageSize() { }

        public PackageSize(int id, string name)
            : base(id, name)
        {

        }

        public static IEnumerable<PackageSize> List()
        {
            return new[] { Envelopes, SmallPackages, MidiunPackages, LargePackages, XLargePackages, OddPackages };
        }

        public static PackageSize FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Possible values for CardType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static PackageSize From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values for CardType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
