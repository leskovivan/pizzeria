using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using pizzeria;
using pizzeria.Interfaces;
using pizzeria.Repository;
using pizzeria.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();

// читаем строку из конфигов и проверяем
var csFromConfig = builder.Configuration.GetConnectionString("DefaultConnection");
var csFromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var cs = !string.IsNullOrWhiteSpace(csFromEnv) ? csFromEnv : csFromConfig;

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Check appsettings.json or environment variable ConnectionStrings__DefaultConnection.");

builder.Services.AddDbContext<ApplicationContext>(o => o.UseSqlServer(cs));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IProduct, ProductRepository>();
builder.Services.AddTransient<IOrder, OrderRepository>();
builder.Services.AddTransient<ICategory, CategoryRepository>();
builder.Services.AddScoped(e => CartRepository.GetCart(e));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("DefaultConnection (env override: {hasEnv}): {cs}", !string.IsNullOrWhiteSpace(csFromEnv), cs);

    var db = sp.GetRequiredService<ApplicationContext>();
    await db.Database.MigrateAsync();

    var userMgr = sp.GetRequiredService<UserManager<User>>();
    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInit.InitializeAsync(userMgr, roleMgr);
    await DbInit.InitializeContentAsync(db);
    await DbInit.CreateSeedDataAsync(db, new[] { 1, 2, 3 });
}

app.Run();
