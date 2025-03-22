using System.ComponentModel.DataAnnotations;

namespace Apollo.Data.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string AvatarUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AuthProvider AuthProvider { get; set; }
}

public enum AuthProvider
{
    Email,
    Google,
}
