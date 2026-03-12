using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HousingAPI.Dtos
{
    public class ResidentDto
    {

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }  // Пароль для нового жильца
        public int ApartmentId { get; set; }
        //сылка на апарты 
        public string? ApartmentLink { get; set; }
        public bool IsOwner { get; set; }

        public string? UserId { get; set; }
    }
}
