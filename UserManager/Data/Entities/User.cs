namespace UserManager.Data.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int? Age { get; set; }
}
