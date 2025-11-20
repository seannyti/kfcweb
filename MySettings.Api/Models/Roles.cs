namespace MySettings.Api.Models;

/// <summary>
/// Role constants for the application
/// </summary>
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly string[] AllRoles = { SuperAdmin, Admin, User };

    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
}


