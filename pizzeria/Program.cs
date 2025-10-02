using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pizzeria;
using pizzeria.Interfaces;
using pizzeria.Repository;
using pizzeria.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IProduct, ProductRepository>();
builder.Services.AddTransient<IOrder, OrderRepository>();
builder.Services.AddScoped(e => CartRepository.GetCart(e));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInit.InitializeAsync(userManager, rolesManager);
    var context = services.GetRequiredService<ApplicationContext>();
    await DbInit.InitializeContentAsync(context);
    await DbInit.CreateSeedDataAsync(context, new int[] { 1, 2, 3 });
}

app.Run();
