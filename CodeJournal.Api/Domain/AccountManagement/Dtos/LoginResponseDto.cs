using System;

namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class LoginResponseDto
{

    public string Email { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public List<string> Roles { get; set; }
    public string UserId { get; set;}

}
