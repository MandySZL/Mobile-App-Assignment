using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolFacilityReport.Models;

[Table("Reports")]
public class Report : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("category")]
    public string Category { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("urgency")]
    public int Urgency { get; set; }

    [Column("status")]
    public string Status { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

