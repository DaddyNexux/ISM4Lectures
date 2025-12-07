using FuckingLectures.ActionFilter;
using FuckingLectures.ActionFilters;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Models.Entities;
using FuckingLectures.Services.Attachment;
using FuckingLectures.Services.AuthServicesFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FuckingLectures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AttachmentsController : ControllerBase
    {
        private IAttachmentsService _attachmentsService;
        private readonly IAuthServices authServices;
        private readonly UserManager<User> _userManager;

        public AttachmentsController(IAttachmentsService attachmentsService, IAuthServices s, UserManager<User> userManager)
        {
            _attachmentsService = attachmentsService;
            authServices = s;
            _userManager = userManager;
        }
        [SwaggerGroup("app", "Dashboard")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("Image")]
        public async Task<ActionResult<ApiResponse<string>>> UploadImage([Required] IFormFile file)
        {
            string result = await _attachmentsService.UploadImages(file);

            var response = ApiResponse<string>.Success(result);

            return StatusCode(response.StatusCode, response);

        }
        [SwaggerGroup("app", "Dashboard")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]
        [HttpPost("file")]
        public async Task<ActionResult<ApiResponse<string>>> UploadFile([Required] IFormFile file)
        {
            string result = await _attachmentsService.UploadFiles(file);

            var response = ApiResponse<string>.Success(result);
            return StatusCode(response.StatusCode, response);

        }
      



        private async Task<User?> GetUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return null;

            // Retrieve the full user from UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;
            return user;
        }
    }
}