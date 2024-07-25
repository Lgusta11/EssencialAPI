using Microsoft.AspNetCore.Identity;

namespace WebAPIUdemy.Repositories.RoleRepositories
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateAsync(IdentityRole role);
    }
}
