using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
    public class CustomProfileService : IProfileService
    {
        public UserManager<ApplicationUser> _user { get; set; }
        public CustomProfileService(UserManager<ApplicationUser> usermanager)
        {
            _user = usermanager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _user.GetUserAsync(context.Subject);
            var ExistingClaims = await _user.GetClaimsAsync(user);

            var newClaim = new List<Claim> {
                new Claim("userName" , user.UserName), // adding the userName to the Token
            };

            context.IssuedClaims.AddRange(newClaim);
            context.IssuedClaims.Add(ExistingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)); // adding the FullName to the Token

        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}