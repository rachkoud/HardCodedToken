using System;

namespace IdentityService
{
    public static class Helper
    {
        public static void ShowIdentity(Identity id)
        {
            Console.WriteLine("Principal: {0}", id.PrincipalType);
            Console.WriteLine("Identity: {0}", id.IdentityType);

            id.Claims.ForEach(c => ShowClaim(c));
        }

        public static void ShowClaim(ClaimDto c)
        {
            Console.WriteLine("\n{0}", c.Type);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" {0}", c.Value);
            Console.ResetColor();
            Console.WriteLine(" {0}", c.Issuer);
        }
    }
}
