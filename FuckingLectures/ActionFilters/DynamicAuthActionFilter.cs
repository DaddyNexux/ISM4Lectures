using FuckingLectures.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FuckingLectures.ActionFilters
{
    public class DynamicAuthActionFilter : IAsyncActionFilter
    {
        private readonly AppData _dbContext;

        public DynamicAuthActionFilter(AppData dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                await next();
                return;
            }

            var user = context.HttpContext.User;

            // If user is not authenticated, block immediately
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get all roles of the user (Identity puts them in ClaimTypes.Role)
            var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (userRoles.Count == 0)
            {
                // No roles assigned, forbid access
                context.Result = new ForbidResult();
                return;
            }

            string controllerName = controllerActionDescriptor.ControllerName;
            string actionName = controllerActionDescriptor.ActionName;


            string requiredPermission = $"{controllerName}.{actionName}".ToLowerInvariant();

            bool hasPermission = await HasPermission(userRoles, requiredPermission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }

        private async Task<bool> HasPermission(List<string> roles, string requiredPermission)
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .Where(r => roles.Contains(r.Name))
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission)
                .AnyAsync(p => p.FullName == requiredPermission);
        }
    }
}
