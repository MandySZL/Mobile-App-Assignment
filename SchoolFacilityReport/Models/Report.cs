using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;

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

    // Helpers for UI Binding
    [JsonIgnore]
    public string ImagePath => ImageUrl;

    [JsonIgnore]
    public Color UrgencyColor
    {
        get
        {
            return Urgency switch
            {
                3 => Colors.Red,        // High
                2 => Colors.Orange,     // Medium
                _ => Colors.Green       // Low
            };
        }
    }

    [JsonIgnore]
    public Color StatusColor
    {
        get
        {
            // Case-insensitive check
            var s = Status?.Trim().ToLower();
            if (s == "completed") return Colors.Green;
            if (s == "in progress") return Colors.Blue;
            if (s == "rejected") return Colors.Red;
            return Colors.Orange; // Pending or others
        }
    }
}