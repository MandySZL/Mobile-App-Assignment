using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolFacilityReport.Models;

[Table("profiles")] // 对应 Supabase 的 profiles 表
public class Profile : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("role")]
    public string Role { get; set; } // Student, Staff, Maintenance
}