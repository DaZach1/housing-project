// Services/HouseService.cs
using HousingAPI.Data;
using HousingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingAPI.Services
{
    public class HouseService
    {
        private readonly HousingContext _context;

        public HouseService(HousingContext context)
        {
            _context = context;
        }

        // Получить все дома с квартирами
        public async Task<List<House>> GetHouses()
        {
            return await _context.Houses
                .Include(h => h.Apartments) // Добавляем загрузку квартир
                .ToListAsync();
        }

        // Получить дом по ID с квартирами
        public async Task<House> GetHouse(int id)
        {
            return await _context.Houses
                .Include(h => h.Apartments) // Добавляем загрузку квартир
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        // Создать дом с квартирами
        public async Task<House> CreateHouse(House house)
        {
            _context.Houses.Add(house);

            // Если есть квартиры - добавляем их связь
            if (house.Apartments?.Count > 0)
            {
                foreach (var apartment in house.Apartments)
                {
                    apartment.HouseId = house.Id; // Устанавливаем связь
                }
            }

            await _context.SaveChangesAsync();
            return house;
        }

        // Добавить квартиру к дому
        public async Task<Apartment> AddApartmentToHouse(int houseId, Apartment apartment)
        {
            var house = await _context.Houses.FindAsync(houseId);
            if (house == null) return null;

            apartment.HouseId = houseId;
            house.Apartments.Add(apartment); // Добавляем через навигационное свойство

            await _context.SaveChangesAsync();
            return apartment;
        }

        // Обновить дом
        public async Task<bool> UpdateHouse(int id, House house)
        {

            if (id != house.Id)
                return false;

            _context.Entry(house).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Удалить дом
        public async Task<bool> DeleteHouse(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Apartments)
                    .ThenInclude(a => a.Residents)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
                return false;

            // Все квартиры и жильцы будут удалены автоматически
            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();
            return true;
        }

        // Проверить существование дома
        public async Task<bool> HouseExists(int id)
        {
            return await _context.Houses.AnyAsync(e => e.Id == id);
        }
    }
}