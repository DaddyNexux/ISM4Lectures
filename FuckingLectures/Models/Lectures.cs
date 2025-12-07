using FuckingLectures.Models.Entities;
using System.Text.Json.Serialization;

namespace FuckingLectures.Models
{
    public class Lectures : BaseEntity
    {
        public string Title { get; set; }
        public Guid CourseId { get; set; }
        [JsonIgnore]
        public Course? Course { get; set; }

        public string LectureUrl { get; set; }

    }

    public class lectureCoeateDTO
    {
               public string Title { get; set; }
        public Guid CourseId { get; set; }
        public string LectureUrl { get; set; }

    }
}
