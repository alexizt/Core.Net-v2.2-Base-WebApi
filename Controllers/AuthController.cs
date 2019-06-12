using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
    readonly DataContext _db;

    public AuthController(IConfiguration configuration, DataContext dataContext)
    {
        _configuration = configuration;
        _db = dataContext;
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
            Claim[] claims = getClaims(user);

            // Generamos el Token
            JwtSecurityToken token = generateToken(claims);

            var _refreshTokenObj = new RefreshToken
            {
                Username = user.Name,
                UserData = JsonConvert.SerializeObject(user),
                Refreshtoken = Guid.NewGuid().ToString()
            };
            _db.RefreshTokens.Add(_refreshTokenObj);
            _db.SaveChanges();

            // Retornamos el token
            return Ok(
                new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = _refreshTokenObj.Refreshtoken
                }
            );
        }
        else
        {
            return Unauthorized();
        }
    }

    //https://localhost:44316/auth/1b5422eb-c738-4950-89d9-5ade1692e5de/refresh/
    [HttpPost("{refreshToken}/refresh")]
    public IActionResult RefreshToken([FromRoute]string refreshToken)
    {
        var _refreshToken = _db.RefreshTokens.SingleOrDefault(m => m.Refreshtoken == refreshToken);

        if (_refreshToken == null)
        {
            return NotFound("Refresh token not found");
        }

        var user = JsonConvert.DeserializeObject<User>(_refreshToken.UserData);
        Claim[] claims = getClaims(user);
        JwtSecurityToken token = generateToken(claims);

        _refreshToken.Refreshtoken = Guid.NewGuid().ToString();
        _db.RefreshTokens.Update(_refreshToken);
        _db.SaveChanges();

        return Ok(
            new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = _refreshToken.Refreshtoken
            });
    }
    
    private JwtSecurityToken generateToken(Claim[] claims)
    {
        return new JwtSecurityToken
        (
            issuer: _configuration["ApiAuth:Issuer"],
            audience: _configuration["ApiAuth:Audience"],
            claims: claims,
            //expires: DateTime.UtcNow.AddDays(60),
            expires: DateTime.UtcNow.AddMinutes(1),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiAuth:SecretKey"])),
                    SecurityAlgorithms.HmacSha256)
        );
    }

    private static Claim[] getClaims(User user)
    {
        return new[] {
                new Claim("UserData", JsonConvert.SerializeObject(user)),
                new Claim("Role", user.Role),
                new Claim("name", user.Name),
                new Claim("IsAdmin", user.Role=="Admin"?"IsAdmin":"NO"),
            };
    }
}