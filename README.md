HardCodedToken
==============

This a POC where a token is created from the ClaimsPrincipal.Current and call a WCF service. The WCF service is configured to receive token from a trusted STS.

## WS-Trust

When we want to secure our WCF service with WS-Trust, we must install a Security Token Service :

![WS-Trust](https://raw.githubusercontent.com/rachkoud/HardCodedToken/master/Documentation/WsTrustDiagram.png)

This setup could be hard to configure and we can't test this in a test environment without the STS, so a workaround is to create an "hardcoded" SAML token from the ClaimsPrincipal.Current and send it with the request to WCF service :

![WS-Trust](https://raw.githubusercontent.com/rachkoud/HardCodedToken/master/Documentation/HardCodedTokenDiagram.png)

So we can start testing our security concepts without the STS and when the STS is configured, the only thing we must change in our code is the call to STS to get the token and only values (no new settings) will change in the web.config.

## Setup

You must change the certificate thumprint value and the hostname.

**Projects from [Thinktecture.IdentityModel.45](https://github.com/thinktecture/Thinktecture.IdentityModel.45) have been used.**
