using Newtonsoft.Json;

namespace DriverDrop.Core.Models.User
{
    public class UserInfo
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }
         
    }
}
