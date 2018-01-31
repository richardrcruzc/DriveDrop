using DriveDrop.Bl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DriveDrop.Bl.Services
{
    public class IdentityParser : IIdentityParser<Models.ApplicationUser>
    {
        public ApplicationUser Parse(IPrincipal principal)
        {

            IIdentity identity = principal.Identity;


            return new ApplicationUser
            {
                Email = identity.Name,
            };
            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
        }
    }
