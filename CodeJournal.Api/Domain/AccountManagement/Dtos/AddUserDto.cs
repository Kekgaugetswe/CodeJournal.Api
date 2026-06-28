using System.ComponentModel.DataAnnotations;

namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class AddUserDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [EmailAddress] 
    public string  Email  { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public bool AdminCheckBox { get; set; }  = false;
}