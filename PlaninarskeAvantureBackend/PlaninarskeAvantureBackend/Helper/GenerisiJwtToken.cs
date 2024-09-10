using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FIT_Api_Example.Helper
{
    public class GenerisiJwtToken
    {
        public static string GenerisiToken()
        {
            DateTime value = DateTime.Now.AddHours(24.0);
            byte[] bytes = Encoding.ASCII.GetBytes("MIIBrTCCAaGg ...");
            SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(bytes), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = value,
                SigningCredentials = signingCredentials
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
