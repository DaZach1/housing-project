using HousingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingAPI.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            HousingContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            Console.WriteLine("▶️ Starting database initialization...");

            try
            {
                // 1. Apply pending migrations (only for relational databases)
                if (!context.Database.IsInMemory())
                {
                    await ApplyMigrations(context);
                }
                else
                {
                    await context.Database.EnsureCreatedAsync();
                    Console.WriteLine("In-memory database created");
                }

                // 2. Initialize roles and users
                await InitializeRolesAndUsers(userManager, roleManager, configuration);

                // 3. Seed housing data if enabled
                if (configuration.GetValue<bool>("DatabaseSettings:SeedData", true))
                {
                    await SeedHousingData(context, userManager, configuration);
                }

                Console.WriteLine("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        private static async Task ApplyMigrations(HousingContext context)
        {
            Console.WriteLine("Checking for pending migrations...");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Migrations applied successfully");
            }
            else
            {
                Console.WriteLine("No pending migrations found");
            }
        }

        private static async Task InitializeRolesAndUsers(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            Console.WriteLine("Initializing roles and users...");

            // Create roles
            string[] roleNames = { "Manager", "Resident" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Role '{roleName}' created");
                }
            }

            // Create test manager
            var managerEmail = configuration["TestUsers:Manager:Email"] ?? "manager@example.com";
            var managerPassword = configuration["TestUsers:Manager:Password"] ?? "Manager123!";
            await CreateUser(userManager, managerEmail, managerPassword, "Manager", "Test Manager", true);

            // Create test resident
            var residentEmail = configuration["TestUsers:Resident:Email"] ?? "resident@example.com";
            var residentPassword = configuration["TestUsers:Resident:Password"] ?? "Resident123!";
            await CreateUser(userManager, residentEmail, residentPassword, "Resident", "Test Resident", false);
        }

        private static async Task CreateUser(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role,
            string fullName,
            bool isManager)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    IsManager = isManager
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    Console.WriteLine($"{role} user '{email}' created successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to create {role} user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"{role} user '{email}' already exists");
            }
        }

        private static async Task SeedHousingData(HousingContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            Console.WriteLine("Seeding housing data...");

            // Check if data already exists
            if (await context.Houses.AnyAsync())
            {
                Console.WriteLine("Housing data already exists, skipping seeding");
                return;
            }

            try
            {
                var baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";

                // 1. Create houses
                var houses = new List<House>
                {
                    new House { Number = 10, Street = "Main St", City = "Riga", Country = "Latvia", PostalCode = "LV-1010" },
                    new House { Number = 25, Street = "Oak Ave", City = "Riga", Country = "Latvia", PostalCode = "LV-1025" }
                };
                await context.Houses.AddRangeAsync(houses);
                await context.SaveChangesAsync();
                Console.WriteLine($"Created {houses.Count} houses");

                // 2. Create apartments
                var apartments = new List<Apartment>
                {
                    new Apartment {
                        Number = 1, Floor = 1, RoomCount = 2, Population = 2,
                        TotalArea = 55.5, LivingArea = 42.3,
                        HouseId = houses[0].Id,
                        HouseLink = $"{baseUrl}/houses/{houses[0].Id}"
                    },
                    new Apartment {
                        Number = 2, Floor = 1, RoomCount = 3, Population = 1,
                        TotalArea = 72.0, LivingArea = 58.5,
                        HouseId = houses[0].Id,
                        HouseLink = $"{baseUrl}/houses/{houses[0].Id}"
                    },
                    new Apartment {
                        Number = 10, Floor = 1, RoomCount = 4, Population = 3,
                        TotalArea = 90.0, LivingArea = 75.0,
                        HouseId = houses[1].Id,
                        HouseLink = $"{baseUrl}/houses/{houses[1].Id}"
                    }
                };
                await context.Apartments.AddRangeAsync(apartments);
                await context.SaveChangesAsync();
                Console.WriteLine($"Created {apartments.Count} apartments");

                // 3. Create residents
                var manager = await context.Users.FirstOrDefaultAsync(u => u.Email == "manager@example.com");
                var resident = await context.Users.FirstOrDefaultAsync(u => u.Email == "resident@example.com");

                var residents = new List<Resident>
                {
                    new Resident {
                        FirstName = "Anna", LastName = "Smith",
                        PersonalCode = "020291-22222", DateOfBirth = new DateTime(1991, 2, 2),
                        Phone = "+371 23456789", Email = "anna@example.com",
                        ApartmentId = apartments[0].Id,
                        UserId = (await CreateUserForResident(
                            userManager,
                            "anna@example.com",
                            "AnnaSmith123!",  // Пароль жильца
                            false)).Id,
                        ApartmentLink = $"{baseUrl}/apartments/{apartments[0].Id}",
                        IsOwner = false
                    },

                    new Resident {
                        FirstName = "Michael", LastName = "Johnson",
                        PersonalCode = "030392-33333", DateOfBirth = new DateTime(1992, 3, 3),
                        Phone = "+371 34567890", Email = "michael@example.com",
                        ApartmentId = apartments[0].Id,
                        UserId = (await CreateUserForResident(
                            userManager,
                            "michael@example.com",
                            "MichaelJ99!",  // Пароль жильца
                            false)).Id,
                        ApartmentLink = $"{baseUrl}/apartments/{apartments[0].Id}",
                        IsOwner = true
                    },

                    new Resident {
                        FirstName = "Emily", LastName = "Williams",
                        PersonalCode = "040493-44444", DateOfBirth = new DateTime(1993, 4, 4),
                        Phone = "+371 45678901", Email = "emily@example.com",
                        ApartmentId = apartments[1].Id,
                        UserId = (await CreateUserForResident(
                            userManager,
                            "emily@example.com",
                            "EmilyPass123!",  // Пароль жильца
                            false)).Id,
                        ApartmentLink = $"{baseUrl}/apartments/{apartments[1].Id}",
                        IsOwner = false
                    },

                    new Resident {
                        FirstName = "David", LastName = "Brown",
                        PersonalCode = "050594-55555", DateOfBirth = new DateTime(1994, 5, 5),
                        Phone = "+371 56789012", Email = "david@example.com",
                        ApartmentId = apartments[1].Id,
                        UserId = (await CreateUserForResident(
                            userManager,
                            "david@example.com",
                            "BrownDavid456!",  // Пароль жильца
                            false)).Id,
                        ApartmentLink = $"{baseUrl}/apartments/{apartments[1].Id}",
                        IsOwner = true
                    },

                    new Resident {
                        FirstName = "Sophia", LastName = "Davis",
                        PersonalCode = "060695-66666", DateOfBirth = new DateTime(1995, 6, 6),
                        Phone = "+371 67890123", Email = "sophia@example.com",
                        ApartmentId = apartments[2].Id,
                        UserId = (await CreateUserForResident(
                            userManager,
                            "sophia@example.com",
                            "SophiaD2023!",  // Пароль жильца
                            false)).Id,
                        ApartmentLink = $"{baseUrl}/apartments/{apartments[2].Id}",
                        IsOwner = false
                    }
                };
                await context.Residents.AddRangeAsync(residents);
                await context.SaveChangesAsync();
                Console.WriteLine($"Created {residents.Count} residents");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding housing data: {ex.Message}");
                throw;
            }
        }
        private static async Task<ApplicationUser> CreateUserForResident(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            bool isOwner)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = email.Split('@')[0],
                IsManager = false
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Resident");
                if (isOwner)
                {
                    await userManager.AddToRoleAsync(user, "Owner");
                }
            }

            return user;
        }
    }
}