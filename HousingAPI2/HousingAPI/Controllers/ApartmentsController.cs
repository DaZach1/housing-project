using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HousingAPI.Services;
using HousingAPI.Models;
using HousingAPI.Dtos;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HousingAPI.Data;

namespace HousingAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : ControllerBase
    {
        private readonly HousingContext _context;
        private readonly IMapper _mapper;
        private readonly ApartmentService _apartmentService;

        public ApartmentsController(HousingContext context, IMapper mapper, ApartmentService apartmentService)
        {
            _context = context;
            _mapper = mapper;
            _apartmentService = apartmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApartmentDto>>> GetApartments()
        {
            IQueryable<Apartment> query = _context.Apartments
                .Include(a => a.House)
                .Include(a => a.Residents);

            if (User.IsInRole("Resident"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                query = query.Where(a => a.Residents.Any(r => r.UserId == userId));
            }

            var apartments = await query.ToListAsync();
            var apartmentDtos = _mapper.Map<IEnumerable<ApartmentDto>>(apartments);

            foreach (var dto in apartmentDtos)
            {
                dto.HouseLink = Url.Action("GetHouse", "Houses",
                    new { id = dto.HouseId },
                    Request.Scheme);
            }

            return Ok(apartmentDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApartmentDto>> GetApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.House)
                .Include(a => a.Residents)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
                return NotFound();

            if (User.IsInRole("Resident"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!apartment.Residents.Any(r => r.UserId == userId))
                    return Forbid();
            }

            var dto = _mapper.Map<ApartmentDto>(apartment);
            dto.HouseLink = Url.Action("GetHouse", "Houses",
                new { id = apartment.HouseId },
                Request.Scheme);

            return Ok(dto);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult<ApartmentDto>> PostApartment(ApartmentDto apartmentDto)
        {
            var houseExists = await _context.Houses.AnyAsync(h => h.Id == apartmentDto.HouseId);
            if (!houseExists)
                return BadRequest("House with specified ID does not exist");

            var apartment = _mapper.Map<Apartment>(apartmentDto);
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<ApartmentDto>(apartment);
            resultDto.HouseLink = Url.Action("GetHouse", "Houses", new { id = apartment.HouseId }, Request.Scheme);

            return CreatedAtAction(nameof(GetApartment), new { id = resultDto.Id }, resultDto);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApartment(int id, [FromBody] ApartmentDto apartmentDto)
        {
            if (id != apartmentDto.Id)
                return BadRequest("ID mismatch");

            if (User.IsInRole("Resident"))
            {
                var apartment = await _context.Apartments
                    .Include(a => a.Residents)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (apartment == null)
                    return NotFound();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!apartment.Residents.Any(r => r.UserId == userId))
                    return Forbid();
            }

            apartmentDto.Residents = null;
            var success = await _apartmentService.UpdateApartment(id, apartmentDto);
            if (!success)
                return BadRequest("Update failed or invalid data");

            return NoContent();
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApartment(int id)
        {
            var success = await _apartmentService.DeleteApartment(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}