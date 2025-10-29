// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using BankApp_Models;

namespace BankApp_Cons;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("===========================================");
        Console.WriteLine("    BankApp Database Setup Tool");
        Console.WriteLine("===========================================\n");

        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=BankAppDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<BankAppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using (var context = new BankAppDbContext(optionsBuilder.Options))
        {
            Console.WriteLine("1. Creating database...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("   ✓ Database created successfully!\n");

            Console.WriteLine("2. Checking for existing data...");
            var userCount = await context.Users.CountAsync();

            if (userCount == 0)
            {
                Console.WriteLine("   No users found. Seeding initial data...\n");
                await SeedInitialData(context);
            }
            else
            {
                Console.WriteLine($"   ✓ Database already contains {userCount} user(s).\n");
            }

            Console.WriteLine("===========================================");
            Console.WriteLine("    Database Setup Complete!");
            Console.WriteLine("===========================================\n");

            Console.WriteLine("Test Accounts:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Admin Account:");
            Console.WriteLine("  Email: admin@bankapp.be");
            Console.WriteLine("  Password: admin123");
            Console.WriteLine("\nUser Account:");
            Console.WriteLine("  Email: user@bankapp.be");
            Console.WriteLine("  Password: user123");
            Console.WriteLine("------------------------------------------------\n");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    static async Task SeedInitialData(BankAppDbContext context)
    {
        try
        {
            // Seed Admin User
            Console.WriteLine("   Creating admin user...");
            var adminUser = new BankUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Admin",
                LastName = "Beheerder",
                Email = "admin@bankapp.be",
                UserName = "admin@bankapp.be",
                NormalizedEmail = "ADMIN@BANKAPP.BE",
                NormalizedUserName = "ADMIN@BANKAPP.BE",
                PhoneNumber = "0470123456",
                DateOfBirth = new DateTime(1980, 1, 1),
                PasswordHash = "admin123", // NIET VEILIG - alleen voor demo
                Role = "Admin",
                ThemePreference = "Light",
                FontSize = 14,
                FontWeight = "Normal",
                HighContrast = false,
                ZoomLevel = 100,
                CreatedAt = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Admin user created\n");

            // Create admin accounts
            Console.WriteLine("   Creating admin accounts...");
            var adminBetaalRekening = new Account
            {
                UserId = adminUser.Id,
                AccountNumber = "123456789",
                IBAN = "BE68123456789012",
                AccountTypeId = 1,
                Balance = 5000.00m,
                Currency = "EUR",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            context.Accounts.Add(adminBetaalRekening);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Admin accounts created\n");

            // Seed Test User
            Console.WriteLine("   Creating test user...");
            var testUser = new BankUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Jan",
                LastName = "Janssens",
                Email = "user@bankapp.be",
                UserName = "user@bankapp.be",
                NormalizedEmail = "USER@BANKAPP.BE",
                NormalizedUserName = "USER@BANKAPP.BE",
                PhoneNumber = "0471234567",
                DateOfBirth = new DateTime(1990, 5, 15),
                PasswordHash = "user123", // NIET VEILIG - alleen voor demo
                Role = "User",
                ThemePreference = "Light",
                FontSize = 14,
                FontWeight = "Normal",
                HighContrast = false,
                ZoomLevel = 100,
                CreatedAt = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true
            };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Test user created\n");

            // Create user accounts
            Console.WriteLine("   Creating user accounts...");
            var userBetaalRekening = new Account
            {
                UserId = testUser.Id,
                AccountNumber = "987654321",
                IBAN = "BE68987654321098",
                AccountTypeId = 1,
                Balance = 2500.00m,
                Currency = "EUR",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var userSpaarRekening = new Account
            {
                UserId = testUser.Id,
                AccountNumber = "111222333",
                IBAN = "BE68111222333444",
                AccountTypeId = 2,
                Balance = 10000.00m,
                Currency = "EUR",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            context.Accounts.AddRange(userBetaalRekening, userSpaarRekening);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ User accounts created\n");

            // Create sample transactions
            Console.WriteLine("   Creating sample transactions...");
            var transaction1 = new Transaction
            {
                FromAccountId = userBetaalRekening.Id,
                ToAccountId = adminBetaalRekening.Id,
                Amount = 50.00m,
                Description = "Testbetaling 1",
                TransactionDate = DateTime.Now.AddDays(-2),
                TransactionType = "Transfer",
                Status = "Completed"
            };

            var transaction2 = new Transaction
            {
                FromAccountId = userBetaalRekening.Id,
                ToAccountId = userSpaarRekening.Id,
                Amount = 500.00m,
                Description = "Overschrijving naar spaarrekening",
                TransactionDate = DateTime.Now.AddDays(-1),
                TransactionType = "Transfer",
                Status = "Completed"
            };

            context.Transactions.AddRange(transaction1, transaction2);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Sample transactions created\n");

            // Create sample investment
            Console.WriteLine("   Creating sample investment...");
            var investment = new Investment
            {
                UserId = testUser.Id,
                AccountId = userSpaarRekening.Id,
                InvestmentTypeId = 1, // Aandelen
                Amount = 1000.00m,
                PurchaseDate = DateTime.Now.AddMonths(-3),
                CurrentValue = 1150.00m,
                ReturnAmount = 150.00m,
                ReturnPercentage = 15.00m,
                IsActive = true
            };
            context.Investments.Add(investment);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Sample investment created\n");

            // Create sample beneficiary
            Console.WriteLine("   Creating sample beneficiary...");
            var beneficiary = new Beneficiary
            {
                UserId = testUser.Id,
                Name = "Maria Peeters",
                IBAN = "BE68555666777888",
                Description = "Zus",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            context.Beneficiaries.Add(beneficiary);
            await context.SaveChangesAsync();
            Console.WriteLine("   ✓ Sample beneficiary created\n");

            Console.WriteLine("   ✓ All initial data seeded successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n   ✗ Error seeding data: {ex.Message}");
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
        }
    }
}
