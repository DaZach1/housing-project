using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HousingAPI.Services;
using HousingAPI.Models;
using HousingAPI.Dtos;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace HousingAPI.Controllers
{
    [Authorize] // Требуется авторизация для всех методов
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ResidentService _residentService;
        private readonly IMapper _mapper;

        public ResidentsController(ResidentService residentService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; 
            _residentService = residentService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Manager")] // Только менеджеры
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResidentDto>>> GetResidents()
        {
            var residents = await _residentService.GetResidents();
            var residentDtos = _mapper.Map<IEnumerable<ResidentDto>>(residents);

            foreach (var dto in residentDtos)
            {
                dto.ApartmentLink = Url.Action("GetApartment", "Apartments", new { id = dto.ApartmentId }, Request.Scheme);
            }

            return Ok(residentDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResidentDto>> GetResident(int id)
        {
            // Если пользователь - жилец, проверяем, что запрашивает себя
            if (User.IsInRole("Resident"))
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var resident = await _residentService.GetResident(id);

                if (resident == null || resident.UserId != currentUserId)
                    return Forbid(); // 403
            }

            // Если пользователь - менеджер или запрос разрешен, возвращаем данные
            var result = await _residentService.GetResident(id);
            if (result == null)
                return NotFound();

            var dto = _mapper.Map<ResidentDto>(result);
            dto.ApartmentLink = Url.Action("GetApartment", "Apartments", new { id = result.ApartmentId }, Request.Scheme);
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ResidentDto>> CreateResident(
            [FromBody] ResidentDto residentDto)
        {
            // Создаем пользователя для жильца
            var user = new ApplicationUser
            {
                UserName = residentDto.Email,
                Email = residentDto.Email,
                FullName = $"{residentDto.FirstName} {residentDto.LastName}",
                IsManager = false
            };

            var result = await _userManager.CreateAsync(user, residentDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Добавляем роль
            await _userManager.AddToRoleAsync(user, "Resident");

            // Создаем жильца
            var resident = _mapper.Map<Resident>(residentDto);
            resident.UserId = user.Id;

            // Проверяем существование квартиры (функциональность из первого метода)
            var createdResident = await _residentService.CreateResident(resident);
            if (createdResident == null)
            {
                // Откатываем создание пользователя, если квартира не существует
                await _userManager.DeleteAsync(user);
                return BadRequest("Apartment with the specified ApartmentId does not exist.");
            }

            var resultDto = _mapper.Map<ResidentDto>(createdResident);
            resultDto.ApartmentLink = Url.Action("GetApartment", "Apartments", new { id = createdResident.ApartmentId }, Request.Scheme);

            return CreatedAtAction(nameof(GetResident), new { id = resultDto.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutResident(int id, [FromBody] ResidentDto residentDto)
        {
            // Проверка для жильцов
            if (User.IsInRole("Resident"))
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var existingResident = await _residentService.GetResident(id);

                if (existingResident == null || existingResident.UserId != currentUserId)
                    return Forbid();
            }

            // Основная логика обновления
            if (id != residentDto.Id)
                return BadRequest("ID mismatch");

            var resident = _mapper.Map<Resident>(residentDto);
            var success = await _residentService.UpdateResident(id, resident);

            if (!success)
                return BadRequest("Update failed");

            return NoContent();
        }

        [Authorize(Roles = "Manager")] // Только менеджеры
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResident(int id)
        {
            var success = await _residentService.DeleteResident(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}