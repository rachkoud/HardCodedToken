using System;
using System.Linq;
using System.Security.Claims;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityModel.Extensions;

namespace IdentityService
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            "\nAuthorization...".ConsoleYellow();

            if (context.Action.First().Type.Equals(ClaimPermission.ActionType))
            {
                Console.WriteLine("Application authorization.");
            }
            else
            {
                Console.WriteLine("WCF pipeline authorization.");
            }

            "\nActions:\n".ConsoleGreen();
            foreach (var action in context.Action)
            {
                Console.WriteLine("{0}\n {1}", action.Type, action.Value);
            }

            "\nResources:\n".ConsoleGreen();
            foreach (var resource in context.Resource)
            {
                Console.WriteLine("{0}\n {1}", resource.Type, resource.Value);
            }

            return true;
        }
    }
}
