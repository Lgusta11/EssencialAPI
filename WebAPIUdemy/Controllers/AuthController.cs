using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPIUdemy.DTOs.AuthDTO;
using WebAPIUdemy.Models;
using WebAPIUdemy.Services;
using WebAPIUdemy.Repositories;

namespace WebAPIUdemy.Controllers;

[Route("[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public AuthController(ITokenService tokenService, IUnitOfWork unitOfWork, IConfiguration config)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _config = config;
    }

    /// <summary>
    /// Realiza o login de um usuário.
    /// </summary>
    /// <param name="model">Modelo contendo o nome de usuário e a senha.</param>
    /// <returns>Um token JWT e um refresh token se o login for bem-sucedido; caso contrário, retorna não autorizado.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /auth/login
    /// {
    ///     "userName": "usuario",
    ///     "password": "senha"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _unitOfWork.UserRepository.FindByNameAsync(model.UserName!);

        if (user is not null && await _unitOfWork.UserRepository.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _unitOfWork.UserRepository.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName!),
                new Claim(ClaimTypes.Email , user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAcessToken(authClaims, _config);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"],
                                            out int RefreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(RefreshTokenValidityInMinutes);

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="model">Modelo contendo os dados do usuário.</param>
    /// <returns>Status de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /auth/register
    /// {
    ///     "userName": "usuario",
    ///     "email": "usuario@example.com",
    ///     "password": "senha"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _unitOfWork.UserRepository.FindByNameAsync(model.UserName!);

        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response { Status = "Error", Message = "User already exists!" });
        }
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName,
        };

        var result = await _unitOfWork.UserRepository.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response { Status = "Error", Message = "User creation failed!" });
        }

        await _unitOfWork.CommitAsync();
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    /// <summary>
    /// Gera um novo token de acesso utilizando um refresh token válido.
    /// </summary>
    /// <param name="tokenModel">Modelo contendo o token de acesso e o refresh token.</param>
    /// <returns>Um novo token de acesso e refresh token.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /auth/refresh-token
    /// {
    ///     "accessToken": "tokenDeAcesso",
    ///     "refreshToken": "tokenDeRefresh"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel == null)
        {
            return BadRequest("Invalid client request");
        }

        string? accessToken = tokenModel.AcessToken
                              ?? throw new ArgumentNullException(nameof(tokenModel));

        string? refreshToken = tokenModel.RefreshToken
                               ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _config);

        if (principal == null)
            return BadRequest("Invalid access token/ refresh token");

        string username = principal.Identity.Name;

        var user = await _unitOfWork.UserRepository.FindByNameAsync(username!);

        if (user == null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token/ refresh token");
        }

        var newAccessToken = _tokenService.GenerateAcessToken(
                                           principal.Claims.ToList(), _config);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga o refresh token de um usuário específico.
    /// </summary>
    /// <param name="username">Nome de usuário cujo refresh token será revogado.</param>
    /// <returns>Status de sucesso ou erro.</returns>
    [HttpPost]
    [Route("revoke/{username}")]
    [Authorize(Policy = "ExclusiveOnly")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _unitOfWork.UserRepository.FindByNameAsync(username);

        if (user == null) return BadRequest("Invalid user name");

        user.RefreshToken = null;

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }

    /// <summary>
    /// Cria um novo papel (role) no sistema.
    /// </summary>
    /// <param name="roleName">Nome do papel a ser criado.</param>
    /// <returns>Status de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /auth/CreateRole
    /// {
    ///     "roleName": "Admin"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("CreateRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _unitOfWork.RoleRepository.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            var roleResult = await _unitOfWork.RoleRepository.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                await _unitOfWork.CommitAsync();
                return StatusCode(StatusCodes.Status200OK,
                    new Response
                    {
                        Status = "Success",
                        Message = $"Role {roleName} added successfully"
                    });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                   new Response
                   {
                       Status = "Error",
                       Message = $"Issue adding the new {roleName} role"
                   });
            }

        }

        return StatusCode(StatusCodes.Status400BadRequest,
        new Response
        {
            Status = "Error",
            Message = $"Role already exist."
        });
    }

    /// <summary>
    /// Adiciona um usuário a um papel específico.
    /// </summary>
    /// <param name="email">Email do usuário a ser adicionado ao papel.</param>
    /// <param name="roleName">Nome do papel ao qual o usuário será adicionado.</param>
    /// <returns>Status de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /auth/AddUserToRole
    /// {
    ///     "email": "usuario@example.com",
    ///     "roleName": "Admin"
    /// }
    /// </remarks>
    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _unitOfWork.UserRepository.FindByEmailAsync(email);

        if (user == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
            new Response
            {
                Status = "Error",
                Message = $"User does not exist."
            });
        }

        var roleExist = await _unitOfWork.RoleRepository.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
            new Response
            {
                Status = "Error",
                Message = $"Role does not exist."
            });
        }

        var result = await _unitOfWork.UserRepository.AddToRoleAsync(user, roleName);

        if (result.Succeeded)
        {
            await _unitOfWork.CommitAsync();
            return StatusCode(StatusCodes.Status200OK,
                new Response
                {
                    Status = "Success",
                    Message = $"User {user.Email} added to {roleName}"
                });
        }

        return StatusCode(StatusCodes.Status400BadRequest,
            new Response
            {
                Status = "Error",
                Message = $"Error occurred while adding user to role."
            });
    }

   
}
