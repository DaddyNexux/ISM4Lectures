using FuckingLectures.Data;
using FuckingLectures.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FuckingLectures.Services.AuthServicesFolder
{
    public interface IRolePermissionService
    {
        Task<ApplicationRole> CreateRoleAsync(string roleName);
        Task<Permission> CreatePermissionAsync(string subject, string action);
        Task<bool> AssignPermissionToRoleAsync(string roleName, string subject, string action);
        Task<bool> RemovePermissionFromRoleAsync(string roleName, string subject, string action);
        Task<List<ApplicationRole>> GetRoles();
        Task<List<Permission>> GetPermissions();
        Task<bool> AssignRoleToUserAsync(Guid userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName);

    }

    public class RolePermissionService : IRolePermissionService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppData _context;
        private readonly UserManager<User> _userManager;
        public RolePermissionService(RoleManager<ApplicationRole> roleManager, AppData context, UserManager<User> usermanager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = usermanager;
        }
        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return false;

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return false;

            if (await _userManager.IsInRoleAsync(user, roleName))
                return true;

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<List<ApplicationRole>> GetRoles()
        {
            return await _roleManager.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task<List<Permission>> GetPermissions()
        {
            return await _context.Permissions.ToListAsync();

        }
        public async Task<ApplicationRole> CreateRoleAsync(string roleName)
        {
            var existingRole = await _roleManager.FindByNameAsync(roleName);
            if (existingRole != null)
                return existingRole;

            var newRole = new ApplicationRole { Name = roleName };
            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
                return newRole;

            throw new Exception("Failed to create role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<Permission> CreatePermissionAsync(string subject, string action)
        {
            var fullName = $"{subject}.{action}".ToLowerInvariant(); // normalized

            var existing = await _context.Permissions
                .FirstOrDefaultAsync(p => p.FullName.ToLower() == fullName);

            if (existing != null)
                return existing;

            var permission = new Permission
            {
                Subject = subject,
                Action = action,
                FullName = fullName
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> AssignPermissionToRoleAsync(string roleName, string subject, string action)
        {
            var permission = await CreatePermissionAsync(subject, action);

            // Include RolePermissions when loading role
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == roleName);


            if (role == null)
                return false;

            bool alreadyAssigned = role.RolePermissions.Any(rp => rp.PermissionId == permission.Id);
            if (alreadyAssigned)
                return true;

            var rolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };

            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(string roleName, string subject, string action)
        {
            var fullName = $"{subject}.{action}".ToLowerInvariant();

            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
                return false;

            var rolePermission = role.RolePermissions
                .FirstOrDefault(rp => rp.Permission.FullName.ToLower() == fullName);

            if (rolePermission == null)
                return false;

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
