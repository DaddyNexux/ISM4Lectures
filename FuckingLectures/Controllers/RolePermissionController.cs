using FuckingLectures.ActionFilter;
using FuckingLectures.ActionFilters;
using FuckingLectures.Models.DTOs;
using FuckingLectures.Services.AuthServicesFolder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FuckingLectures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RolePermissionController(IRolePermissionService rolePermissionService,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _rolePermissionService = rolePermissionService;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            try
            {
                var role = await _rolePermissionService.CreateRoleAsync(roleName);
                return Ok(new { message = "Role created successfully", role });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("create-permission")]
        public async Task<IActionResult> CreatePermission([FromQuery] string subject, [FromQuery] string action)
        {
            try
            {
                var permission = await _rolePermissionService.CreatePermissionAsync(subject, action);
                return Ok(new { message = "Permission created successfully", permission });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("assign-permission")]
        public async Task<IActionResult> AssignPermission([FromQuery] string roleName, [FromQuery] string subject, [FromQuery] string action)
        {
            var result = await _rolePermissionService.AssignPermissionToRoleAsync(roleName, subject, action);
            if (result)
                return Ok(new { message = "Permission assigned to role" });
            return BadRequest(new { error = "Failed to assign permission to role" });
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpDelete("remove-permission")]
        public async Task<IActionResult> RemovePermission([FromQuery] string roleName, [FromQuery] string subject, [FromQuery] string action)
        {
            var result = await _rolePermissionService.RemovePermissionFromRoleAsync(roleName, subject, action);
            if (result)
                return Ok(new { message = "Permission removed from role" });
            return BadRequest(new { error = "Failed to remove permission from role" });
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _rolePermissionService.GetRoles();
            return Ok(roles);
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpGet("get-permissions")]
        public async Task<IActionResult> Getpermissions()
        {
            var permissions = await _rolePermissionService.GetPermissions();
            return Ok(permissions);
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("AddUserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] UserAssignRoleDTO dto)
        {
            var success = await _rolePermissionService.AssignRoleToUserAsync(dto.UserId, dto.RoleName);

            if (!success)
                return BadRequest("Failed to assign role. Either user or role not found, or already assigned.");

            return Ok("Role assigned successfully.");
        }
        [SwaggerGroup("Dashboard", "App")]

        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("RemoveUserRole")]
        public async Task<IActionResult> RemoveUserRole([FromBody] UserAssignRoleDTO dto)
        {
            var success = await _rolePermissionService.RemoveRoleFromUserAsync(dto.UserId, dto.RoleName);

            if (!success)
                return BadRequest("Failed to remove role. Either user or role not found, or already removed.");

            return Ok("Role removed successfully.");
        }
        [SwaggerGroup("Dashboard", "App")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]

        [HttpGet("GetAllActions")]
        public IActionResult GetAllActions()
        {
            var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(a => new
                {
                    Controller = a.ControllerName,
                    Action = a.ActionName,
                    Route = string.Join("/", a.AttributeRouteInfo?.Template ?? "")
                })
                .OrderBy(a => a.Controller)
                .ThenBy(a => a.Action)
                .ToList();

            return Ok(actions);
        }
        [SwaggerGroup("Dashboard", "App")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]

        [HttpPost("AllPermsToAdmin")]
        public async Task<IActionResult> AllPermsToAdmin()
        {
            var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .OfType<ControllerActionDescriptor>()
                .Select(a => new
                {
                    Controller = a.ControllerName,
                    Action = a.ActionName,
                    Route = string.Join("/", a.AttributeRouteInfo?.Template ?? "")
                })
                .OrderBy(a => a.Controller)
                .ThenBy(a => a.Action)
                .ToList();

            foreach (var act in actions)
            {
                await _rolePermissionService.AssignPermissionToRoleAsync("superadmin", act.Controller, act.Action);
            }
            return Ok(new { message = "All permissions assigned to superadmin." });
        }
    }
}
