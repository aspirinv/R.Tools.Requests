using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using R.Tools.Extensions;
using R.Tools.Requests.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace R.Tools.Requests.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public const string Key = "9QySQvdHEjx7CBLmL3KYCV88hDewiKw74XfO69033Zkf0a12iaGtwjQ3WLx3zHTw";
        [HttpPost]
        [AuthFormatter]
        public ActionResult<AuthResponse> Authenticate(AuthRequest login)
        {
            var key = Encoding.ASCII.GetBytes(Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("Id", login.login),
                ]),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new AuthResponse(tokenHandler.WriteToken(token)));
        }
    }

    public record AuthRequest(string login, string password);
    public record AuthResponse(string auth_token);
    public class AuthFormatter : ContentFormatter<AuthRequest>
    {
        public override object FixObject(AuthRequest body)
        {
            return new { body.login };
        }
    }
}
