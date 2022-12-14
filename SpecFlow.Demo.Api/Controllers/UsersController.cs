using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpecFlow.Demo.Api.Entities;
using SpecFlow.Demo.Api.Models;
using SpecFlow.Demo.Api.Models.Auth;
using SpecFlow.Demo.Api.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace SpecFlow.Demo.Api.Controllers;

[ApiController]
[Route("user")]
[Produces("application/json")]
[Consumes("application/json")]
public class UsersController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly JwtConfig _jwtConfig;

    public UsersController(DataContext dataContext, IOptions<JwtConfig> jwtConfig)
    {
        _dataContext = dataContext;
        _jwtConfig = jwtConfig.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token response", typeof(TokenResponseModel))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<TokenResponseModel>> LoginAsync([FromBody] UserLoginModel request)
    {
        var user = await AuthenticateAsync(request.Email, request.Password);

        if (user != null)
        {
            var token = GenerateToken(user.Id, user.Email, user.Name);
            return Ok(token);
        }

        return NotFound("User not found");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token response", typeof(TokenResponseModel))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<TokenResponseModel>> RegisterAsync([FromBody] UserRegisterModel request)
    {
        var emailIsUsed = await _dataContext.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (emailIsUsed)
        {
            return BadRequest("An user with this email is already in use");
        }

        var newUser = await _dataContext.Users.AddAsync(new User
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name
        });

        await _dataContext.SaveChangesAsync();

        return Ok(GenerateToken(newUser.Entity.Id, newUser.Entity.Email, newUser.Entity.Password));

    }

    private TokenResponseModel GenerateToken(Guid id, string email, string name)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Sid, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.GivenName, name)
        };

        var expirationDate = DateTime.Now.AddMinutes(15).ToUniversalTime();
        var token = new JwtSecurityToken(_jwtConfig.Issuer, _jwtConfig.Audience,
            claims,
            expires: expirationDate,
            signingCredentials: credentials);

        return new TokenResponseModel(id, name, new JwtSecurityTokenHandler().WriteToken(token), expirationDate);
    }

    private async Task<UserModel> AuthenticateAsync(string email, string password)
    {
        var currentUser = await _dataContext.Users.FirstOrDefaultAsync(o => o.Email.ToLower() == email.ToLower() && o.Password == password);

        if (currentUser != null)
        {
            return new UserModel(currentUser.Id, currentUser.Email, currentUser.Name);
        }

        return null;
    }
}