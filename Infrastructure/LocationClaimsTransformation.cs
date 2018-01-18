using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Users.Infrastructure
{
    public class LocationClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.HasClaim(p => p.Type != ClaimTypes.PostalCode))
            {
                if (principal.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.IsAuthenticated && claimsIdentity.Name != null)
                {
                    if (claimsIdentity.Name.ToLower() == "yj")
                    {
                        claimsIdentity.AddClaims(new List<Claim>
                        {
                            new Claim(ClaimTypes.PostalCode, "DC 20500", ClaimValueTypes.String, "RemoteClaims"),
                            new Claim(ClaimTypes.StateOrProvince, "DC", ClaimValueTypes.String, "RemoteClaims")
                        });
                    }
                    else
                    {
                        claimsIdentity.AddClaims(new List<Claim>
                        {
                            new Claim(ClaimTypes.PostalCode, "NY 10036", ClaimValueTypes.String, "RemoteClaims"),
                            new Claim(ClaimTypes.StateOrProvince, "NY", ClaimValueTypes.String, "RemoteClaims")
                        });
                    }
                }
            }
            return Task.FromResult(principal);
        }
    }
}