namespace CodeJournal.Api.Domain.AccountManagement.Dtos;

public class AssignRoleDto
{
    public string UserIdOrEmail { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}