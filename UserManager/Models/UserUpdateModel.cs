using System.ComponentModel.DataAnnotations;

namespace UserManager.Models;

public class UserUpdateModel
{
    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int? Age { get; set; }
}
