namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class AddUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string  Email  { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool AdminCheckBox { get; set; }  = false;
}