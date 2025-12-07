using FuckingLectures.Data;
using FuckingLectures.Helpers;
using FuckingLectures.Models.DTOs.Common;
using FuckingLectures.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FuckingLectures.Models.DTOs.Auth;

namespace FuckingLectures.Services.AuthServicesFolder
{
    public interface IAuthServices
    {
        Task<ApiResponse<LoginResDTO?>> Login(LoginReqDTO form);
        Task<ApiResponse<string>> Register(RegisterDTO form);



    }
    public class AuthService : IAuthServices
    {
        private readonly AppData _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AuthService(AppData context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<LoginResDTO?>> Login(LoginReqDTO form)
        {
            if (form is null)
                return ApiResponse<LoginResDTO?>.Fail("Invalid request", 400);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u =>
                    (form.Username != null && u.UserName == form.Username) ||
                    (form.phoneNumber != null && u.PhoneNumber == form.phoneNumber));

            if (user is null)
                return ApiResponse<LoginResDTO?>.Fail("Invalid Credentials", 401);

            if (!await _userManager.CheckPasswordAsync(user, form.Password))
                return ApiResponse<LoginResDTO?>.Fail("Invalid Credentials", 401);

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User"; // prevent null

            var secretKey = ConfigProvider.config["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                return ApiResponse<LoginResDTO?>.Fail("Server configuration error", 500);

            var token = JwtToken.GenToken(user.Id, userRole, "supernova-iq.com", 30, secretKey);

            var roleEntity = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == userRole);

            var permissions = roleEntity?.RolePermissions?
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission.FullName)
                .ToList() ?? new List<string>();

            var response = new LoginResDTO
            {
                Token = token,
                Id = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Role = userRole,
                Email = user.Email,

                Permissions = permissions
            };

            return ApiResponse<LoginResDTO?>.Success(response, "Login Successful");
        }


        public async Task<ApiResponse<string>> Register(RegisterDTO form)
        {
            var user = new User
            {
                UserName = form.Username,
                PhoneNumber = form.PhoneNumber,
                Email = form.Email,
                FullName = form.FullName,


            };
            var result = await _userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return ApiResponse<string>.Fail($"User registration failed: {errors}", 400);
            }
            var roleExists = await _roleManager.RoleExistsAsync(Roles.User);
            if (!roleExists)
            {
                return ApiResponse<string>.Fail($"Role '{Roles.User}' does not exist.", 400);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!addRoleResult.Succeeded)
            {
                string errorMsg = "Failed to assign role: " + string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                return ApiResponse<string>.Fail(errorMsg, 400);
            }

            return ApiResponse<string>.Success(user.PhoneNumber, "Registration complete", 200);
        }
      
       




    }
}
