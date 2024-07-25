using Microsoft.AspNetCore.Identity;
using WebAPIUdemy.Models;

namespace WebAPIUdemy.Repositories.UserRepositories;

public interface IUserRepository
{
    Task<ApplicationUser> FindByNameAsync(string userName);
    Task<ApplicationUser> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
}
