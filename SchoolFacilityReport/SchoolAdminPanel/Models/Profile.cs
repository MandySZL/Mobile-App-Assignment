using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace SchoolAdminPanel.Models;

[Table("profiles")]
public class Profile : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("role")]
    public string Role { get; set; }
}
