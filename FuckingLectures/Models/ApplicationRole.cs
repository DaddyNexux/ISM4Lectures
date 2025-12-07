using Microsoft.AspNetCore.Identity;

namespace FuckingLectures.Models.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();


    }
}
