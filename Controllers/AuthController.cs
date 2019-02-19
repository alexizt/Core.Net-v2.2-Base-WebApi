using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //https://localhost:5001/auth/login?UserId=admin&Password=admin
    [HttpPost]
    [Route("[action]")]
    public IActionResult Login(LoginViewModel model)
    {
        /*
         * Aquí deberá ir su lógica de validación, en nuestro ASP.NET Identity
         * y luego de verificar que sea una autenticación correcta vamos a proceder a
         * generar el token
         */

        User user = null;

        if (model.UserId == "admin" && model.Password == "admin")
        {
            // Asumamos que tenemos un usuario válido
            user = new User
            {
                Name = "Alexis",
                Role = "Admin",
                UserId = "admin"
            };
        }

        if (model.UserId == "user" && model.Password == "user")
        {
            // Asumamos que tenemos un usuario válido
            user = new User
            {
                Name = "Usuario",
                Role = "User",
                UserId = "user"
            };
        }



        if (user != null)
        {

            /* Creamos la información que queremos transportar en el token,
             * en nuestro los datos del usuario
             */
            var claims = new[] {
                new Claim("UserData", JsonConvert.SerializeObject(user)),
                new Claim("Role", user.Role),
                new Claim("name", user.Name),
                new Claim("IsAdmin", user.Role=="Admin"?"IsAdmin":"NO"),
            };

            // Generamos el Token
            var token = new JwtSecurityToken
            (
                issuer: _configuration["ApiAuth:Issuer"],
                audience: _configuration["ApiAuth:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiAuth:SecretKey"])),
                        SecurityAlgorithms.HmacSha256)
            );

            // Retornamos el token
            return Ok(
                new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                }
            );
        }
        else
        {
            return Unauthorized();
        }
    }
}