using System.ComponentModel.DataAnnotations;

namespace MyUsers.Api.DTOs;

/// <summary>
/// Request to update user role (Admin/SuperAdmin only)
/// </summary>
public class UpdateRoleRequest
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public required string Role { get; set; }
}

