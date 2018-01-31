
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DriveDrop.Bl.ViewModels
{

    public class RateModel
    {
        public int Id { get; set; }
        public decimal OverHead { get;   set; }
        public PackageSizeModel PackageSize { get;   set; }

        public List<RatePriorityModel> RatePriorities { get;   set; }

    } 

    public class RateModelOl
    { 
        public int Id { get; set; } 
        public decimal OverHead { get; set; }  
        public TypeModel PackageSize { get; set; } 
        public List<RatePriorityModel> RatePriorities { get; set; }
        
    }


    public class RatePriorityModel
    { 
        public int PriorityTypeId { get; set; } 
        public TypeModel PriorityType { get; set; } 
        public decimal Charge { get; set; } 
        public bool ChargePercentage { get; set; }

        public int RateId { get; set; }
    }

    public class TypeModel
    {
        public string Name { get;  set; }

        public int Id { get;  set; }

    }

    public class WeightAndDistance
    {
        public List<RateDetailModel> RateWeightSizeModel { get; set; }

        public List<RateDetailModel> RateDistanceModel { get; set; }

    }

    public class RateDetailModel
    {
        public int Id { get; set; }
        public string WeightOrDistance { get; set; }
        public string MileOrLbs { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Charge { get; set; }

        public int RateId { get; set; }
    }

    

    public class RateDeleteDetailModel
    {
        public int RateId { get; set; }
        public List<RateDetailModel> RateDetails { get; set; }
        //public List<RatePackageSizeModel> RatePackageSizes { get; set; }
        public List<RatePriorityModel> RatePriorities { get; set; }

    }
}
