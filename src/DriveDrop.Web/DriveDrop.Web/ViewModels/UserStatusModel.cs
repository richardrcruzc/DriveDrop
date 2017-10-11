
namespace DriveDrop.Web.ViewModels
{
    public class UserStatusModel
    {
        public string Status { get; set; }        
        public string Impersonated { get; set; }
        public int CustomerTypeId { get; set; }
        
             public string CustomerType { get; set; }
        public string UserName { get; set; }

        public string LastName { get; set; }
        
    }
}
