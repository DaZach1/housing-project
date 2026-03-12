using System.Text.Json.Serialization;

namespace HousingAPI.Models
{
    //Созадем базу с домам 
    public class House
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        [JsonIgnore]
        public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}
