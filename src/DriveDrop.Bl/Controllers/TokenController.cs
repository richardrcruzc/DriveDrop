using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DriveDrop.Bl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DriveDrop.Bl.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class TokenController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public TokenController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get(string username, string password, string grant_type)
        {
            {
                var user = await userManager.FindByEmailAsync(username);

                if (user != null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
                    if (result.Succeeded)
                    {

                        var claims = new[]
                        {
                         new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim( JwtRegisteredClaimNames.Sub, username),
                        new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim( JwtRegisteredClaimNames.GivenName, "SomeUserID")
                    };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretesecretesecretesecretesecretesecrete"));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(issuer: "test",
                                                         audience: "test",
                                                         claims: claims,
                                                          expires: DateTime.Now.AddDays(15),
                                                          signingCredentials: creds);

                        return Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token), expires_on = DateTime.Now.AddDays(15) });

                    }
                }
            }

            return BadRequest("Could not create token");
        }


    }
}