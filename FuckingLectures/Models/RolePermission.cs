using System.Text.Json.Serialization;

namespace FuckingLectures.Models.Entities
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        [JsonIgnore] public ApplicationRole? Role { get; set; }

        public Guid PermissionId { get; set; }
        [JsonIgnore] public Permission? Permission { get; set; }
    }
}
