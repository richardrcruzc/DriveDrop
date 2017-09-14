using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Mobile.Models
{
    public class User
    {
        string id;

        public string Id
        {
            get
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return id;
                }
                else if (!string.IsNullOrEmpty(Email))
                {
                    return Email;
                }
                else
                {
                    return Username;
                }
            }
            set
            {
                id = value;
            }
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
