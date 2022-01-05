using JBNWebAPI.Logger;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace JBNWebAPI
{
    public class JBNAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string username = "dMRo3oqVLOs=", password = "Dgy7t7MvU3JZx+ObnW6z3A==";

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            try
            {
                if (Helper.Decrypt(context.UserName, "sblw-3hn8-sqoy19") == Helper.Decrypt(username, "sblw-3hn8-sqoy19") && Helper.Decrypt(context.Password, "sblw-3hn8-sqoy19") == Helper.Decrypt(password, "sblw-3hn8-sqoy19"))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                    identity.AddClaim(new Claim("username", Helper.Decrypt(context.UserName, "sblw-3hn8-sqoy19")));
                    identity.AddClaim(new Claim(ClaimTypes.Name, Helper.Decrypt(context.UserName, "sblw-3hn8-sqoy19")));
                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "Invalid Username and Password");
                    return;
                }
            }
            catch(Exception ex)
            {
                context.SetError("invalid_grant", "Invalid Username and Password");
                return;
            }
            
        }
    }
}