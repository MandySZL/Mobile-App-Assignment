using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace WebAdmin.Models;

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

    // Web Helpers
    public string ImagePath => ImageUrl;

    public string UrgencyColor
    {
        get
        {
            return Urgency switch
            {
                3 => "red",        // High
                2 => "orange",     // Medium
                _ => "green"       // Low
            };
        }
    }

    public string StatusBadgeClass
    {
        get
        {
            var s = Status?.Trim().ToLower();
            if (s == "completed") return "bg-success";
            if (s == "in progress") return "bg-primary";
            if (s == "rejected") return "bg-danger";
            return "bg-warning text-dark"; // Pending or others
        }
    }
}
