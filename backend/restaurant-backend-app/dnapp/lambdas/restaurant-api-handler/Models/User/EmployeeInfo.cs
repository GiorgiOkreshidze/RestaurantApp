namespace Function.Models.User;

public class EmployeeInfo
{
    public required string Email { get; set; }
    
    public required string LocationId { get; set; }
    
    public required Roles Role { get; set; }
}