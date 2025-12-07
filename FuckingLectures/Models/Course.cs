using FuckingLectures.Models.Entities;

namespace FuckingLectures.Models
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructor { get; set; }

        public ICollection<Lectures>? Lectures { get; set; } = new List<Lectures>();
    }

    public class CourseCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructor { get; set; }
    }
}
