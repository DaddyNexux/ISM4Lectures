using FuckingLectures.ActionFilters;
using FuckingLectures.Models;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuckingLectures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly ILectureServices _lectureServices;

        public LectureController(ILectureServices lectureServices)
        {
            _lectureServices = lectureServices;
        }

        // GET: api/Lecture
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _lectureServices.GetAsync();
            return StatusCode(response.StatusCode, response);
        }

        // GET: api/Lecture/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _lectureServices.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // POST: api/Lecture
        [HttpPost]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Create([FromBody] lectureCoeateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid data", 400));

            var response = await _lectureServices.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // PUT: api/Lecture/{id}
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Update(Guid id, [FromBody] lectureCoeateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid data", 400));

            var response = await _lectureServices.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE: api/Lecture/{id}
        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _lectureServices.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
