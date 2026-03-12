using HousingAPI.Models;
using System.Text.Json.Serialization;

public class Apartment
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Floor { get; set; }
    public int RoomCount { get; set; }
    public int Population { get; set; }
    public double TotalArea { get; set; }
    public double LivingArea { get; set; }

    public int HouseId { get; set; }
    public string? HouseLink { get; set; }

    [JsonIgnore]
    public virtual House? House { get; set; }

    [JsonIgnore]
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
}