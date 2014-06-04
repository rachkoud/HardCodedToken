using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.ServiceModel;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityModel.Extensions;

namespace IdentityService
{
    [ServiceBehavior(Name = "ClaimsService", Namespace = "urn:tt")]
    public class ClaimsService : IClaimsService
    {
        [ClaimPermission(SecurityAction.Demand,
            Operation = "Read", Resource = "Claims")]
        public Identity GetIdentity()
        {
            "\n\nRequest message:".ConsoleYellow();
            Console.WriteLine(OperationContext.Current.RequestContext.RequestMessage.ToString());

            var principal = ClaimsPrincipal.Current;

            var result = ClaimsAuthorization.CheckAccess(
                "Read",
                "Claims",
                "IdentityType",
                "PrincipalType");
            
            var id = new Identity
            {
                PrincipalType = principal.GetType().FullName,
                IdentityType = principal.Identity.GetType().FullName,
                
                Claims = new List<ClaimDto>(
                    from claim in principal.Claims
                    select new ClaimDto
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                        Issuer = claim.Issuer,
                        OriginalIssuer = claim.OriginalIssuer,
                    })
            };

            return id;
        }
    }
}