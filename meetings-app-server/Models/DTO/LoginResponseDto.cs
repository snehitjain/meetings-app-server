namespace meetings_app_server.Models.DTO;
public class LoginResponseDto
{
    public string AuthToken { get; set; }
    public string? Email { get; set; }
    //public string Role { get; set; }
}
