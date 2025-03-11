using API.DTOs;

namespace API.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterUser(RegisterUserDto userDto);

        Task<LoginResponseDto> LoginUser(LoginDTO loginDTO);
    }
}
