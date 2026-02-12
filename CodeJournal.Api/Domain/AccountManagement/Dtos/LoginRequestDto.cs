using System;

namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class LoginRequestDto
{
   
    public string Email { get; set; }
    public string Password { get; set; }

}
