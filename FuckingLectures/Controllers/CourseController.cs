using FuckingLectures.ActionFilters;
using FuckingLectures.Models;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuckingLectures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseServices _courseServices;

        public CourseController(ICourseServices courseServices)
        {
            _courseServices = courseServices;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseServices.GetAsync();
            return StatusCode(response.StatusCode, response);
        }

        // GET: api/Course/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _courseServices.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        // POST: api/Course
        [HttpPost]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Create([FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid data", 400));

            var response = await _courseServices.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        // PUT: api/Course/{id}
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid data", 400));

            var response = await _courseServices.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        // DELETE: api/Course/{id}
        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _courseServices.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
