// Services/ResidentService.cs
using HousingAPI.Data;
using HousingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HousingAPI.Services
{
    public class ResidentService
    {
        private readonly HousingContext _context;

        public ResidentService(HousingContext context)
        {
            _context = context;
        }

        // Получить всех жильцов
        public async Task<List<Resident>> GetResidents()
        {
            return await _context.Residents.ToListAsync();
        }

        // Получить жильца по ID
        public async Task<Resident> GetResident(int id)
        {
            return await _context.Residents.FindAsync(id);
        }
        public async Task<Resident> GetResidentByUserId(string userId)
        {
            return await _context.Residents.FirstOrDefaultAsync(r => r.UserId == userId);
        }
        // Создать жильца
        public async Task<Resident> CreateResident(Resident resident)
        {
            // Проверка существования квартиры
            var apartmentExists = await _context.Apartments.AnyAsync(a => a.Id == resident.ApartmentId);
            if (!apartmentExists)
                return null;

            // Проверяем существование пользователя, если UserId указан
            if (!string.IsNullOrEmpty(resident.UserId))
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == resident.UserId);
                if (!userExists) return null;
            }

            _context.Residents.Add(resident);
            await _context.SaveChangesAsync();
            return resident;
        }

        // Обновить жильца
        public async Task<bool> UpdateResident(int id, Resident updatedResident)
        {
            // 1. Находим существующую запись
            var existingResident = await _context.Residents
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingResident == null)
                return false;

            // 2. Обновляем только изменяемые поля
            _context.Entry(existingResident).CurrentValues
                .SetValues(updatedResident);

            // 3. Явно помечаем изменённые поля (опционально)
            _context.Entry(existingResident).Property(x => x.FirstName).IsModified = true;
            _context.Entry(existingResident).Property(x => x.LastName).IsModified = true;
            // ... остальные изменяемые поля

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Concurrency error updating resident");
                return false;
            }
        }

        // Удалить жильца
        public async Task<bool> DeleteResident(int id)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
                return false;

            _context.Residents.Remove(resident);
            await _context.SaveChangesAsync();
            return true;
        }

        // Проверить существование жильца
        public async Task<bool> ResidentExists(int id)
        {
            return await _context.Residents.AnyAsync(e => e.Id == id);
        }
    }
}