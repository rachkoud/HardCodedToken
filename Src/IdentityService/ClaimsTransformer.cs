using System;
using System.Linq;
using System.Security.Claims;
using Thinktecture.IdentityModel.Extensions;

namespace IdentityService
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            "\nClaims transformation....\n".ConsoleYellow();

            incomingPrincipal.Identities.First().AddClaim(new Claim("http://claims/custom/time", DateTime.Now.ToLongTimeString()));
            return incomingPrincipal;
        }
    }
}
