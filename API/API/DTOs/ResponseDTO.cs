namespace API.DTOs
{
    public class ResponseDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public ResponseDto(string msg, bool isSuccess)
        {
            Message = msg;
            IsSuccess = isSuccess;
        }
    }

    public class LoginResponseDto
    {
        public string Message { get; set; }
        public string Token{ get; set; }
        public bool IsSuccess { get; set; }
        public LoginResponseDto(string msg, string token, bool isSuccess)
        {
            Message = msg;
            Token = token;
            IsSuccess = isSuccess;
        }
    }
}
