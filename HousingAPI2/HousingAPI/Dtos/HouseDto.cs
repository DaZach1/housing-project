namespace HousingAPI.Dtos
{
    public class HouseDto
    {

        public int Id { get; set; }
        public int Number { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public ICollection<ApartmentDto>? Apartments { get; set; }
    }
}