using IdentityService;
using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.WSTrust;

namespace Client
{
    class Program
    {
        // Change the hostname!
        static Uri _serviceAddress = new Uri("https://windows7:444/identity/wstrust");
        // You can set your identity server if you want to request a security token from it
        static EndpointAddress _idsrvAddress = new EndpointAddress(new Uri("https://windows7/idsrv/issue/wstrust/mixed/username"));

        static void Main(string[] args)
        {
            SetCurrentIdentity();
            //var token = RequestSecurityToken();
            var token = GenerateHardcodedToken();
            CallService(token);
        }

        private static void SetCurrentIdentity()
        {
            var claimsIdentity = new ClaimsIdentity("CustomAuthentication");
            claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "bob"));
            claimsIdentity.AddClaim(new Claim("http://identityserver.thinktecture.com/claims/profileclaims/twittername", ""));
            claimsIdentity.AddClaim(new Claim("http://identityserver.thinktecture.com/claims/profileclaims/city", ""));
            claimsIdentity.AddClaim(new Claim("http://identityserver.thinktecture.com/claims/profileclaims/homepage", ""));

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            Thread.CurrentPrincipal = claimsPrincipal;
        }

        private static SecurityToken RequestSecurityToken()
        {
            "Requesting identity token".ConsoleYellow();


            var factory = new WSTrustChannelFactory(new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential), _idsrvAddress);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.UserName.UserName = "bob";
            factory.Credentials.UserName.Password = "abc!123";

            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,
                AppliesTo = new EndpointReference(_serviceAddress.AbsoluteUri + "/bearer")
            };

            return factory.CreateChannel().Issue(rst);
        }

        private static SigningCredentials GenerateSigningCredentials()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var x509Certificate2 = store.Certificates.Find(X509FindType.FindByThumbprint, "4443BE13F1B67C4D2733FFBF100E4F114B08FFBF", false).OfType<X509Certificate2>().FirstOrDefault();
            
            // Both signatures work
            var signingToken = new X509AsymmetricSecurityKey(x509Certificate2);
            var securityKeyIdentifier1 = new SecurityKeyIdentifier(new X509RawDataKeyIdentifierClause(x509Certificate2));
            var signingCredentials1 = new SigningCredentials(signingToken, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest, securityKeyIdentifier1);

            //RSACryptoServiceProvider rsa = x509Certificate2.PrivateKey as RSACryptoServiceProvider;
            //RsaSecurityKey rsaKey = new RsaSecurityKey(rsa);
            //X509RawDataKeyIdentifierClause x509RawClause = new X509RawDataKeyIdentifierClause(x509Certificate2);
            //SecurityKeyIdentifier securityKeyIdentifier2 = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] { x509RawClause });
            //SigningCredentials signingCredentials2 = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha1Signature, SecurityAlgorithms.Sha1Digest, securityKeyIdentifier2);

            return signingCredentials1;
        }

        private static SecurityToken GenerateHardcodedToken()
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor();
            securityTokenDescriptor.Subject = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            securityTokenDescriptor.Lifetime = new Lifetime(DateTime.Now, DateTime.Now.AddDays(2));
            securityTokenDescriptor.TokenIssuerName = "http://identityserver.v2.thinktecture.com/trust/changethis";
            securityTokenDescriptor.AppliesToAddress = "https://windows7:444/identity/wstrust/bearer";
            securityTokenDescriptor.SigningCredentials = GenerateSigningCredentials();

            Saml2SecurityTokenHandler saml2SecurityTokenHandler = new Saml2SecurityTokenHandler();
            var saml2SecurityToken = saml2SecurityTokenHandler.CreateToken(securityTokenDescriptor) as Saml2SecurityToken;

            var authenticationMethod = "urn:oasis:names:tc:SAML:2.0:ac:classes:Password";
            var authenticationContext = new Saml2AuthenticationContext(new Uri(authenticationMethod));
            saml2SecurityToken.Assertion.Statements.Add(new Saml2AuthenticationStatement(authenticationContext));

            return saml2SecurityToken;
        }

        private static void CallService(SecurityToken token)
        {
            "Calling Service".ConsoleYellow();

            var binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.EstablishSecurityContext = false;
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;

            var factory = new ChannelFactory<IClaimsService>(binding, new EndpointAddress(_serviceAddress));
            factory.Credentials.SupportInteractive = false;
            factory.Credentials.UseIdentityConfiguration = true;
            var proxy = factory.CreateChannelWithIssuedToken(token);

            var id = proxy.GetIdentity();
            Helper.ShowIdentity(id);
        }
    }
}
