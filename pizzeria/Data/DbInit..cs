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
            if (!await db.Products.AnyAsync())
            {
                var p = new List<Product>
                {
                    new Product { Name="Пицца Маргарита", Description="Классическая пицца", Weight=500, Calories=800, Price=199, Brand="PizzaStar", Type=ProductType.Dish, CategoryId=1, Image="/productFiles/pizza.png", DateOfPublication=DateTime.Now },
                    new Product { Name="Салат Цезарь", Description="Салат с курицей", Weight=300, Calories=450, Price=149, Brand="PizzaStar", Type=ProductType.Dish, CategoryId=2, Image="/productFiles/salad.png", DateOfPublication=DateTime.Now },
                    new Product { Name="Лимонад", Description="Освежающий напиток", Weight=500, Calories=120, Price=69, Brand="PizzaStar", Type=ProductType.Drink, CategoryId=3, Image="/productFiles/drink.png", DateOfPublication=DateTime.Now }
                };
                await db.Products.AddRangeAsync(p);
                await db.SaveChangesAsync();
            }
        }

        public static async Task CreateSeedDataAsync(ApplicationContext db, int[] categories)
        {
            foreach (var c in categories)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (!await db.Products.AnyAsync(x => x.Name == $"Товар {c}-{i}"))
                    {
                        await db.Products.AddAsync(new Product
                        {
                            Name = $"Товар {c}-{i}",
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
    }
}
