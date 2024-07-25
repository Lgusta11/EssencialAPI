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

    [HttpPost]
    [Route("AddUserToRole")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _unitOfWork.UserRepository.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _unitOfWork.UserRepository.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                  new Response
                  {
                      Status = "Success",
                      Message = $"User {user.Email} added to the {roleName} role"
                  });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                  new Response
                  {
                      Status = "Error",
                      Message = $"Unable to add user {user.Email} to the {roleName} role"
                  });
            }
        }
        return BadRequest(
            new
            {
                error = "Unable to find user"
            });
    }
}
