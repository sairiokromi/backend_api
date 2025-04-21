using Kromi.Application.Contracts.Services;
using Microsoft.AspNetCore.Identity;

namespace Kromi.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public string[] ListadoRoles()
            => _roleManager.Roles.Select(s => s.Name).ToArray()!;
    }
}
