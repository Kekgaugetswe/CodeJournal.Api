using System;

namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class RegisterRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

}
