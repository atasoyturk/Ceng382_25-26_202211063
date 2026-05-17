using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;
using tastemam.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<LogService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddHttpClient<ChatbotService>();

builder.Services.AddScoped<PdfService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://maps.googleapis.com; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https://maps.googleapis.com https://maps.gstatic.com; " +
        "frame-ancestors 'none';");
    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    string[] roles = { "Admin", "Caretaker", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    if (await userManager.FindByEmailAsync("atasoyturkk@tastemam.com") == null)
    {
        var admin = new IdentityUser { UserName = "atasoyturkk@tastemam.com", Email = "atasoyturkk@tastemam.com" };
        await userManager.CreateAsync(admin, "adminata");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    if (await userManager.FindByEmailAsync("efenayin@tastemam.com") == null)
    {
        var caretaker = new IdentityUser { UserName = "efenayin@tastemam.com", Email = "efenayin@tastemam.com" };
        await userManager.CreateAsync(caretaker, "caretakerefe");
        await userManager.AddToRoleAsync(caretaker, "Caretaker");
    }

    if (await userManager.FindByEmailAsync("dogand@tastemam.com") == null)
    {
        var user = new IdentityUser { UserName = "dogand@tastemam.com", Email = "dogand@tastemam.com" };
        await userManager.CreateAsync(user, "userdogan");
        await userManager.AddToRoleAsync(user, "User");
    }

    if (!context.MenuItems.Any())
    {
        var caretaker = await userManager.FindByEmailAsync("efenayin@tastemam.com");
        context.MenuItems.AddRange(
            new tastemam.Models.Menu
            {
                Name = "Düğün Menüsü Klasik",
                Description = "Çorba, ana yemek, tatlı ve içecek dahil komple düğün menüsü.",
                Price = 350,
                Category = "Düğün",
                CaretakerID = caretaker.Id,
                ImagePath = ""

            },
            new tastemam.Models.Menu
            {
                Name = "Kurumsal Toplantı Menüsü",
                Description = "Sandviç, meyve tabağı, çay ve kahve dahil hafif kurumsal menü.",
                Price = 150,
                Category = "Kurumsal",
                CaretakerID = caretaker.Id,
                ImagePath = ""

            },
            new tastemam.Models.Menu
            {
                Name = "Doğum Günü Özel Menü",
                Description = "Pasta, atıştırmalıklar ve içecekler dahil eğlenceli doğum günü menüsü.",
                Price = 200,
                Category = "Özel Gün",
                CaretakerID = caretaker.Id,
                ImagePath = ""
            }
        );
        await context.SaveChangesAsync();
    }
}

app.Run();