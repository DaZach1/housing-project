// Services/ApartmentService.cs
using AutoMapper;
using HousingAPI.Data;
using HousingAPI.Dtos;
using HousingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingAPI.Services
{
    public class ApartmentService
    {
        private readonly HousingContext _context;
        private readonly IMapper _mapper;
        public ApartmentService(HousingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Получить все квартиры
        public async Task<List<Apartment>> GetApartments()
        {
            return await _context.Apartments.ToListAsync();
        }

        // Получить квартиру по ID
        public async Task<Apartment> GetApartment(int id)
        {
            return await _context.Apartments.FindAsync(id);
        }

        // Создать квартиру
        public async Task<Apartment> CreateApartment(ApartmentDto apartmentDto)
        {
            var apartment = _mapper.Map<Apartment>(apartmentDto);

            var houseExists = await _context.Houses.AnyAsync(h => h.Id == apartment.HouseId);
            if (!houseExists)
                return null;

            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            return apartment;
        }

        // Обновить квартиру
        public async Task<bool> UpdateApartment(int id, ApartmentDto apartmentDto)
        {
            var existingApartment = await _context.Apartments.FindAsync(id);
            if (existingApartment == null)
                return false;

            // Проверяем существование дома
            if (!await _context.Houses.AnyAsync(h => h.Id == apartmentDto.HouseId))
                return false;

            // Обновляем только необходимые поля
            existingApartment.Number = apartmentDto.Number;
            existingApartment.Floor = apartmentDto.Floor;
            existingApartment.RoomCount = apartmentDto.RoomCount;
            existingApartment.Population = apartmentDto.Population;
            existingApartment.TotalArea = apartmentDto.TotalArea;
            existingApartment.LivingArea = apartmentDto.LivingArea;
            existingApartment.HouseId = apartmentDto.HouseId;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating apartment: {ex.Message}");
                return false;
            }
        }

        // Удалить квартиру
        public async Task<bool> DeleteApartment(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Residents)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
                return false;

            // Все жильцы будут удалены автоматически
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}