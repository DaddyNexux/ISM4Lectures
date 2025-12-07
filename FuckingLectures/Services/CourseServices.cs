using FuckingLectures.Data;
using FuckingLectures.Helpers;
using FuckingLectures.Models;
using FuckingLectures.Models.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace FuckingLectures.Services
{
    public interface ICourseServices
    {
        Task<ApiResponse<Course>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<Course>>> GetAsync();
        Task<ApiResponse<Course>> CreateAsync(CourseCreateDto CreateDTO);
        Task<ApiResponse<Course>> UpdateAsync(Guid Id, CourseCreateDto UpdateDTO);
        Task<ApiResponse<bool>> DeleteAsync(Guid Id);
    }
    public class CourseServices : ICourseServices
    {
        private readonly AppData _context;
        public CourseServices(AppData context)
        {

            _context = context;
        }

        public async Task<ApiResponse<Course>> GetByIdAsync(Guid id)
        {
            var course = await _context.Course.Include(c => c.Lectures)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return ApiResponse<Course>.Fail("Course not found", 404);
            return ApiResponse<Course>.Success(course);

        }

        public async Task<ApiResponse<List<Course>>> GetAsync()
        {
            var courses = await _context.Course.Include(c => c.Lectures)
                .Where(c => !c.IsDeleted).ToListAsync();
            return ApiResponse<List<Course>>.Success(courses);
        }

        public async Task<ApiResponse<Course>> CreateAsync(CourseCreateDto CreateDTO)
        {
            var course = new Course
            {
                Title = CreateDTO.Title,
                Description = CreateDTO.Description,
                Instructor = CreateDTO.Instructor
            };
            _context.Course.Add(course);
            await _context.SaveChangesAsync();
            return ApiResponse<Course>.Success(course);
        }

        public async Task<ApiResponse<Course>> UpdateAsync(Guid Id, CourseCreateDto UpdateDTO)
        {
            var course = await _context.Course.FirstOrDefaultAsync(c => c.Id == Id);
            if (course == null)
                return ApiResponse<Course>.Fail("Course not found", 404);
            course.Title = UpdateDTO.Title;
            course.Description = UpdateDTO.Description;
            course.Instructor = UpdateDTO.Instructor;
            _context.Course.Update(course);
            await _context.SaveChangesAsync();
            return ApiResponse<Course>.Success(course);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid Id)
        {
            var course = await _context.Course.FirstOrDefaultAsync(c => c.Id == Id);
            if (course == null)
                return ApiResponse<bool>.Fail("Course not found", 404);
            await course.Delete(_context);
            _context.Course.Update(course);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true);
        }
    }
}
