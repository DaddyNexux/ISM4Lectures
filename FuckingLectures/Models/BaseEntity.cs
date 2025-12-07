using FuckingLectures.Data;
using System.ComponentModel.DataAnnotations;

namespace FuckingLectures.Models.Entities;

public class BaseEntity
{
    [Key] public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public virtual async Task Delete(AppData _context)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    public virtual async Task UnDelete(AppData _context)
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}