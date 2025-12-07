using FuckingLectures.ActionFilter;
using FuckingLectures.ActionFilters;
using FuckingLectures.Models.DTOs.Auth;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Models.Entities;
using FuckingLectures.Services.AuthServicesFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FuckingLectures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly UserManager<User> _userManager;
        public AuthController(IAuthServices authServices, UserManager<User> userManager)
        {
            _authServices = authServices;
            _userManager = userManager;
        }
        [SwaggerGroup("app", "Dasboard")]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResDTO?>>> Login([FromBody] LoginReqDTO form)
        {
            var response = await _authServices.Login(form);
            
            return StatusCode(response.StatusCode, response);
        }
        [SwaggerGroup("app", "Dasboard")]
        [ServiceFilter(typeof(DynamicAuthActionFilter))]

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterDTO form)
        {
            var response = await _authServices.Register(form);
            if(response.StatusCode == 200)
            {
                return StatusCode(response.StatusCode, response);

            }

            return StatusCode(response.StatusCode, response);
        }

      


      
       


        private async Task<User?> GetUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return null;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user;
        }


    }
}
