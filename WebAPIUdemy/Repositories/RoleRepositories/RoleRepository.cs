using Microsoft.AspNetCore.Identity;

namespace WebAPIUdemy.Repositories.RoleRepositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role)
    {
        return await _roleManager.CreateAsync(role);
    }
}
