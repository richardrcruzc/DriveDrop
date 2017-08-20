
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DriveDrop.Web.ViewModels
{
    public class RateModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("overHead")]
        public decimal OverHead { get; set; } 
        [JsonProperty("packageSize")]
        public TypeModel PackageSize { get; set; }
        [JsonProperty("ratePriorities")]
        public List<RatePriorityModel> RatePriorities { get; set; }
        
    }


    public class RatePriorityModel
    {
        [JsonProperty("priorityTypeId")]
        public int PriorityTypeId { get; set; }
        [JsonProperty("priorityType")]
        public TypeModel PriorityType { get; set; }
        [JsonProperty("charge")]
        public decimal Charge { get; set; }
        [JsonProperty("chargePercentage")]
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
