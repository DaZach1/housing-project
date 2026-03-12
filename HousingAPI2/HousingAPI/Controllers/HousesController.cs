using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HousingAPI.Services;
using HousingAPI.Models;
using HousingAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HousingAPI.Data;

namespace HousingAPI.Controllers
{
    [Authorize(Roles = "Manager")] // Только менеджеры имеют доступ
    [Route("api/[controller]")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HousingContext _context;
        private readonly IMapper _mapper;
        private readonly HouseService _houseService;

        public HousesController(HouseService houseService, HousingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _houseService = houseService;
        }

        [AllowAnonymous] // Разрешаем доступ без авторизации
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HouseDto>>> GetHouses()
        {
            var houses = await _context.Houses
                .Include(h => h.Apartments)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<HouseDto>>(houses));
        }

        [AllowAnonymous] // Разрешаем доступ без авторизации
        [HttpGet("{id}")]
        public async Task<ActionResult<HouseDto>> GetHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Apartments)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null) return NotFound();

            return Ok(_mapper.Map<HouseDto>(house));
        }

        [HttpPost]
        public async Task<ActionResult<House>> PostHouse(House house)
        {
            var createdHouse = await _houseService.CreateHouse(house);
            return CreatedAtAction(nameof(GetHouse), new { id = createdHouse.Id }, createdHouse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutHouse(int id, House house)
        {
            if (id != house.Id)
                return BadRequest();

            var success = await _houseService.UpdateHouse(id, house);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            var success = await _houseService.DeleteHouse(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}