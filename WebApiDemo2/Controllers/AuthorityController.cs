using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApiDemo2.Authority;

namespace WebApiDemo2.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        private readonly IConfiguration configuration;

        //gia na paroume to security key tha xreiastoume ena dependency injection edw 
        public AuthorityController(IConfiguration configuration) //injecting the configuration sto controller
        {
            this.configuration = configuration;
        }


        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody]AppCredential credential) //dilwnoume body giati alliws tha mporouse na einai sto head kai den theloume na einai sto head giati o kathe xristis tha mporouse na to dei 
        {
            if (Authenticator.Authenticate(credential.ClientId, credential.Secret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    //o logos pou dinoume to client Id einai gia otan dimiourgoume to token an einai ena json web  toke tha perilembanei thn pliroforia gia to aplication
                    access_token =Authenticator.CreateToken(credential.ClientId, expiresAt, configuration["SecurityKey"]?? string.Empty), //to create token prepei na kaleite apo ton authenticator kai to configuration to pername mesa apo authn thn methodo
                    expires_at = expiresAt
                    //kai tote to clientId mporei na xrisimopoeithei gia na paroume tin aplication pliroforia opote epistrefoume auto
                });
            }
            else
            {
                //if the authentication fails
                //formating the error se kati sigkekrimeno
                ModelState.AddModelError("Unauthorized", "You are not authorized.");
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };
                return new UnauthorizedObjectResult(problemDetails);
            }
        }
    }
}
