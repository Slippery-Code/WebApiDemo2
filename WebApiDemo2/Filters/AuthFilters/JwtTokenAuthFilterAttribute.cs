using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WebApiDemo2.Attributes;
using WebApiDemo2.Authority;

namespace WebApiDemo2.Filters.AuthFilters
{

    public class JwtTokenAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {

        /* autocompleted generated code apo visual studio 
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //implement your JWT token validation logic here
            // If the token is invalid, set context.Result to an UnauthorizedResult or similar
            //example: Context.Result = new UnauthorizedResult();
            
            await Task.CompletedTask;
        }
        */

        //implement the interface
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //kanoume intercept the http request to prwto pragma pou prepei na kanome einai to authorization header

            //1.Get Authorization header from the request
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            string tokenString = authorizationHeader.ToString();
            //2. Get rid of the Bearer prefix
            if (tokenString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) //epeidh to Bearer exei keno to sumperilambanoume sto string
            {
                tokenString = tokenString.Substring("Bearer ".Length).Trim(); //kai meta trimaroume to Bearer me to keno gia na paroume auto pou theloume
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            //3.Get Configuration where we store the security key and the Secret key
            //pernoume to secret(security) key apo to configuration 
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var securityKey = configuration?["SecurityKey"] ?? string.Empty;

            //4.Verify the token 
            /*
            if (await Authenticator.VerifyTokenAsync(tokenString, securityKey))
            {
                //token is valid, continue with the request
                return;
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }*/

            //final 4. verify the token and extract the claims
            var claims = await Authenticator.VerifyTokenAsync(tokenString, securityKey);
            if (claims == null)
            {
                context.Result = new UnauthorizedResult(); //401
                //an den exoume ta claims tote leme oti den einai authorized
            }
            else
            {
                //get the claims requirment
                var requiredClaims = context.ActionDescriptor.EndpointMetadata.
                    OfType<RequiredClaimAttribute>()
                    .ToList(); //to toList ta epistrefei ola
                //trying to check wheather each reuired is actually present within the actual claims here
                //if the second codition an i timi tou value einai isi me to claim value within the required claim
                //opote auto simenei oti kathe ena claim mesa sto required claim yparxeime to claim 
                if (requiredClaims != null && requiredClaims.All
                    (rc => claims.Any(c => c.Type.Equals(rc.ClaimType, StringComparison.OrdinalIgnoreCase)
                    && c.Value.Equals(rc.ClaimValue, StringComparison.OrdinalIgnoreCase))))
                {
                    //All required claims are present
                    context.Result = new StatusCodeResult(403); //forbidden
                }
            }


        }
    }
}
