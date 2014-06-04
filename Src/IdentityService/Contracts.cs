using System.Collections.Generic;
using System.ServiceModel;

namespace IdentityService
{
    [ServiceContract(Name = "ClaimsServiceContract", Namespace = "urn:tt")]
    public interface IClaimsService
    {
        [OperationContract(Name = "GetIdentity", Action = "GetIdentity", ReplyAction = "GetIdentityReply")]
        Identity GetIdentity();
    }

    public class Identity
    {
        public string PrincipalType { get; set; }
        public string IdentityType { get; set; }
        public List<ClaimDto> Claims { get; set; }
    }

    public class ClaimDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Issuer { get; set; }
        public string OriginalIssuer { get; set; }
    }
}
