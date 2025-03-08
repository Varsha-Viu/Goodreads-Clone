using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Interfaces;
using API.Modals;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class AuthServices : IAuthService
    {
        private readonly UserManager<Users> _userManager;
        private readonly IConfiguration _configuration;
        public AuthServices(UserManager<Users> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ResponseDto> RegisterUser(RegisterUserDto userDto)
        {
            var userByEmail = await _userManager.FindByEmailAsync(userDto.Email);
            var userByUsername = await _userManager.FindByNameAsync(userDto.UserName);
            if (userByEmail is not null)
                return new ResponseDto("User with email already exists", false);

            if (userByUsername is not null)
                return new ResponseDto("User with this username already exists", false);

            Users user = new()
            {
                Email = userDto.Email,
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.FirstName,
                Address = userDto.Address,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                // add role
                var currentUser = await _userManager.FindByNameAsync(userDto.UserName);
                if (currentUser is not null)
                {
                    var addRole = await _userManager.AddToRoleAsync(currentUser, userDto.Role.ToString());
                }
                return new ResponseDto("User registeres successfully. Please Login to use the site", true);
            }

            return new ResponseDto("Error while registering", false);
        }

        public async Task<LoginResponseDto> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null)
            {
                return new LoginResponseDto("User not found", "", false);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateToken(authClaims);

            var response = new LoginResponseDto("Login successfully", token, true);

            return response;
        }

        private string GenerateToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token); // Convert token to string
        }


        private string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }
}
