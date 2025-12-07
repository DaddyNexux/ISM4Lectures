using FuckingLectures.Data;
using FuckingLectures.Models;
using FuckingLectures.Models.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace FuckingLectures.Services
{
    public interface ILectureServices
    {
        Task<ApiResponse<Lectures>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<Lectures>>> GetAsync();
        Task<ApiResponse<Lectures>> CreateAsync(lectureCoeateDTO CreateDTO);
        Task<ApiResponse<Lectures>> UpdateAsync(Guid Id, lectureCoeateDTO UpdateDTO);
        Task<ApiResponse<bool>> DeleteAsync(Guid Id);
    }
    public class LectureServices : ILectureServices
    {
        private readonly AppData _context;
        public LectureServices(AppData context)
        {
            _context = context;

        }

        public async Task<ApiResponse<Lectures>> GetByIdAsync(Guid id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);
            if (lecture == null)
                return ApiResponse<Lectures>.Fail("Lecture not found", 404);
            return ApiResponse<Lectures>.Success(lecture);

        }

        public async Task<ApiResponse<List<Lectures>>> GetAsync()
        {
                       var lectures = await _context.Lectures.ToListAsync();
            return ApiResponse<List<Lectures>>.Success(lectures);
        }


        public async Task<ApiResponse<Lectures>> CreateAsync(lectureCoeateDTO CreateDTO)
        {
            var lecture = new Lectures
            {
                Title = CreateDTO.Title,
                CourseId = CreateDTO.CourseId,
                LectureUrl = CreateDTO.LectureUrl
            };
            _context.Lectures.Add(lecture);
            await _context.SaveChangesAsync();
            return ApiResponse<Lectures>.Success(lecture);
        }
        public async Task<ApiResponse<Lectures>> UpdateAsync(Guid Id, lectureCoeateDTO UpdateDTO)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == Id);
            if (lecture == null)
                return ApiResponse<Lectures>.Fail("Lecture not found", 404);
            lecture.Title = UpdateDTO.Title;
            lecture.CourseId = UpdateDTO.CourseId;
            lecture.LectureUrl = UpdateDTO.LectureUrl;
            _context.Lectures.Update(lecture);
            await _context.SaveChangesAsync();
            return ApiResponse<Lectures>.Success(lecture);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid Id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == Id);
            if (lecture == null)
                return ApiResponse<bool>.Fail("Lecture not found", 404);
            _context.Lectures.Remove(lecture);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true);
        }


    }
}
