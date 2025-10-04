using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pizzeria.Models;

namespace pizzeria
{
    public static class DbInit
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@pizzeria.com";
            string password = "Admin123!";

            if (await roleManager.FindByNameAsync("Admin") == null)
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (await roleManager.FindByNameAsync("Editor") == null)
                await roleManager.CreateAsync(new IdentityRole("Editor"));

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        public static async Task InitializeContentAsync(ApplicationContext db)
        {
            var needCats = new[] { "Пицца", "Салаты", "Напитки" };
            foreach (var name in needCats)
                if (!await db.Categories.AnyAsync(c => c.Name == name))
                    await db.Categories.AddAsync(new Category { Name = name });
            await db.SaveChangesAsync();

            var catPizzaId = (await db.Categories.FirstAsync(c => c.Name == "Пицца")).Id;
            var catSaladId = (await db.Categories.FirstAsync(c => c.Name == "Салаты")).Id;
            var catDrinkId = (await db.Categories.FirstAsync(c => c.Name == "Напитки")).Id;

            if (!await db.Products.AnyAsync())
            {
                var list = new List<Product>
        {
            new Product { Name="Пицца Маргарита", Description="Классическая пицца", Weight=500, Calories=800, Price=199, Brand="PizzaStar", Type=ProductType.Dish, CategoryId=catPizzaId, Image="/productFiles/pizza.png", DateOfPublication=DateTime.Now },
            new Product { Name="Салат Цезарь", Description="Салат с курицей", Weight=300, Calories=450, Price=149, Brand="PizzaStar", Type=ProductType.Dish, CategoryId=catSaladId, Image="/productFiles/salad.png", DateOfPublication=DateTime.Now },
            new Product { Name="Лимонад", Description="Освежающий напиток", Weight=500, Calories=120, Price=69, Brand="PizzaStar", Type=ProductType.Drink, CategoryId=catDrinkId, Image="/productFiles/drink.png", DateOfPublication=DateTime.Now }
        };
                await db.Products.AddRangeAsync(list);
                await db.SaveChangesAsync();
            }
        }


        public static async Task CreateSeedDataAsync(ApplicationContext db, int[] categories)
        {
            foreach (var c in categories)
            {
                var categoryExists = await db.Categories.AnyAsync(x => x.Id == c);
                if (!categoryExists) continue;

                for (int i = 1; i <= 3; i++)
                {
                    var name = $"Товар {c}-{i}";
                    if (await db.Products.AnyAsync(x => x.Name == name)) continue;

                    await db.Products.AddAsync(new Product
                    {
                        Name = name,
                        Description = "Описание",
                        Weight = 250 + i * 10,
                        Calories = 200 + i * 50,
                        Price = 100 + i * 10,
                        Brand = "Demo",
                        Type = i % 2 == 0 ? ProductType.Drink : ProductType.Dish,
                        CategoryId = c,
                        Image = i % 2 == 0 ? "/productFiles/drink.png" : "/productFiles/pizza.png",
                        DateOfPublication = DateTime.Now
                    });
                }
            }
            await db.SaveChangesAsync();
        }

        public static async Task ClearData(ApplicationContext db)
        {
            await db.ShopCartItems.ExecuteDeleteAsync();
            await db.OrderDetails.ExecuteDeleteAsync();
            await db.Orders.ExecuteDeleteAsync();
            await db.Products.ExecuteDeleteAsync();
        }

        static async Task<Dictionary<string, int>> EnsureCategoriesAsync(ApplicationContext db, IEnumerable<string> names)
        {
            foreach (var name in names)
                if (!await db.Categories.AnyAsync(c => c.Name == name))
                    await db.Categories.AddAsync(new Category { Name = name });

            await db.SaveChangesAsync();

            return await db.Categories
                .Where(c => names.Contains(c.Name))
                .ToDictionaryAsync(c => c.Name, c => c.Id);
        }
    }
}
