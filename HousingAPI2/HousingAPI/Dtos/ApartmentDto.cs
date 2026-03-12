using System.Text.Json.Serialization;

namespace HousingAPI.Dtos
{
    public class ApartmentDto
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

        // Изменения здесь:
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ResidentDto>? Residents { get; set; }
    }
}