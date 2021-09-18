using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestProject.WebAPI
{
    public class AuthOptions
    {
        public const string ISSUER = "http://localhost:44350";
        public const string AUDIENCE = "http://localhost:4200/";
        const string KEY = "superSecretKey@345";
        public const int LIFETIME = 1;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
