using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HousingAPI.Models
{
    //Создаем базу с жителем
    public class Resident
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int ApartmentId { get; set; }
        public string? ApartmentLink { get; set; }
        public bool IsOwner { get; set; } = false;
        [JsonIgnore]
        public virtual Apartment? Apartment { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
}
