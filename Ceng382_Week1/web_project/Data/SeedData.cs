using Microsoft.AspNetCore.Identity;
using tastemam.Models;

namespace tastemam.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // Roller
            string[] roles = { "Admin", "Caretaker", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin
            if (await userManager.FindByEmailAsync("atasoyturkk@gmail.com") == null)
            {
                var admin = new IdentityUser { UserName = "atasoyturkk@gmail.com", Email = "atasoyturkk@gmail.com", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "adminata");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // User
            if (await userManager.FindByEmailAsync("atasoyturkk+user@gmail.com") == null)
            {
                var user = new IdentityUser { UserName = "atasoyturkk+user@gmail.com", Email = "atasoyturkk+user@gmail.com", EmailConfirmed = true };
                await userManager.CreateAsync(user, "userata");
                await userManager.AddToRoleAsync(user, "User");
            }

            // Demo caretaker
            if (await userManager.FindByEmailAsync("atasoyturkk+caretaker@gmail.com") == null)
            {
                var caretaker = new IdentityUser { UserName = "atasoyturkk+caretaker@gmail.com", Email = "atasoyturkk+caretaker@gmail.com", EmailConfirmed = true };
                await userManager.CreateAsync(caretaker, "caretakerata");
                await userManager.AddToRoleAsync(caretaker, "Caretaker");
            }

            // Ankara ilçeleri
            var districts = new[]
            {
                ("altindag", "Altındağ", 39.9470, 32.8627),
                ("cankaya", "Çankaya", 39.9045, 32.8616),
                ("etimesgut", "Etimesgut", 39.9571, 32.6782),
                ("golbasi", "Gölbaşı", 39.7917, 32.8083),
                ("kecioren", "Keçiören", 39.9972, 32.8597),
                ("mamak", "Mamak", 39.9297, 32.9255),
                ("pursaklar", "Pursaklar", 40.0322, 32.8992),
                ("sincan", "Sincan", 39.9739, 32.5826),
                ("yenimahalle", "Yenimahalle", 39.9650, 32.7833),
                ("akyurt", "Akyurt", 40.1338, 33.0808),
                ("ayash", "Ayaş", 40.0167, 32.3333),
                ("bala", "Bala", 39.5583, 33.1167),
                ("beypazari", "Beypazarı", 40.1681, 31.9206),
                ("camlidere", "Çamlıdere", 40.4925, 32.4897),
                ("cubuk", "Çubuk", 40.2333, 33.0333),
                ("elmadag", "Elmadağ", 39.9167, 33.2333),
                ("evren", "Evren", 39.0206, 33.5261),
                ("gudul", "Güdül", 40.2167, 32.2333),
                ("haymana", "Haymana", 39.4333, 32.4978),
                ("kalecik", "Kalecik", 40.0944, 33.4092),
                ("kazan", "Kazan", 40.2028, 32.6878),
                ("kizilcahamam", "Kızılcahamam", 40.4681, 32.6508),
                ("nallihan", "Nallıhan", 40.1833, 31.3500),
                ("polatli", "Polatlı", 39.5833, 32.1469),
                ("sereflikochisar", "Şereflikoçhisar", 38.9347, 33.5358)
            };

            foreach (var (code, name, lat, lng) in districts)
            {
                var email = $"caretaker.{code}@tastemam.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var caretaker = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(caretaker, "Caretaker123!");
                    await userManager.AddToRoleAsync(caretaker, "Caretaker");

                    var districtUser = await userManager.FindByEmailAsync(email);

                    var menus = new List<Menu>
                    {
                        new Menu { Name = $"{name} Düğün Menüsü", Description = $"{name} bölgesine özel düğün catering hizmeti.", Price = 350, Category = "Düğün", CaretakerID = districtUser.Id, ImagePath = "", Latitude = lat, Longitude = lng, MinOrderQuantity = 50 },
                        new Menu { Name = $"{name} Kurumsal Menü", Description = $"{name} bölgesine özel kurumsal catering hizmeti.", Price = 150, Category = "Kurumsal", CaretakerID = districtUser.Id, ImagePath = "", Latitude = lat, Longitude = lng, MinOrderQuantity = 20 },
                        new Menu { Name = $"{name} Özel Gün Menüsü", Description = $"{name} bölgesine özel gün catering hizmeti.", Price = 200, Category = "Özel Gün", CaretakerID = districtUser.Id, ImagePath = "", Latitude = lat, Longitude = lng, MinOrderQuantity = 25 },
                        new Menu { Name = $"{name} Mezuniyet Menüsü", Description = $"{name} bölgesine özel mezuniyet catering hizmeti.", Price = 250, Category = "Mezuniyet", CaretakerID = districtUser.Id, ImagePath = "", Latitude = lat, Longitude = lng, MinOrderQuantity = 30 },
                        new Menu { Name = $"{name} Kokteyl Menüsü", Description = $"{name} bölgesine özel kokteyl catering hizmeti.", Price = 180, Category = "Kokteyl", CaretakerID = districtUser.Id, ImagePath = "", Latitude = lat, Longitude = lng, MinOrderQuantity = 15 }
                    };

                    context.MenuItems.AddRange(menus);
                    await context.SaveChangesAsync();

                    foreach (var menu in menus)
                    {
                        if (menu.Category == "Düğün")
                        {
                            context.Ingredients.AddRange(
                                new Ingredient { MenuID = menu.ID, Name = "Mercimek Çorbası", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Izgara Köfte (Yanında Köz Patlıcan)", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Karışık Salata", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Trilece Tatlısı", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Coca Cola (330ml)", IsRemovable = true }
                            );
                        }
                        else if (menu.Category == "Kurumsal")
                        {
                            context.Ingredients.AddRange(
                                new Ingredient { MenuID = menu.ID, Name = "Domates Çorbası", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Tavuk Şiş (Yanında Pilav)", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Mevsim Salata", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Sütlaç", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Su (500ml)", IsRemovable = false }
                            );
                        }
                        else if (menu.Category == "Özel Gün")
                        {
                            context.Ingredients.AddRange(
                                new Ingredient { MenuID = menu.ID, Name = "Ezogelin Çorbası", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Kuzu İncik (Yanında Bulgur Pilavı)", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Gavurdağı Salata", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Çikolatalı Sufle", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Limonata", IsRemovable = true }
                            );
                        }
                        else if (menu.Category == "Mezuniyet")
                        {
                            context.Ingredients.AddRange(
                                new Ingredient { MenuID = menu.ID, Name = "Yayla Çorbası", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Fırın Tavuk (Yanında Patates)", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Çoban Salata", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Baklava", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Ayran", IsRemovable = true }
                            );
                        }
                        else if (menu.Category == "Kokteyl")
                        {
                            context.Ingredients.AddRange(
                                new Ingredient { MenuID = menu.ID, Name = "Karışık Meze Tabağı", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Mini Sandviç Çeşitleri", IsRemovable = false },
                                new Ingredient { MenuID = menu.ID, Name = "Meyve Tabağı", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Kurabiyelik Tatlı Çeşitleri", IsRemovable = true },
                                new Ingredient { MenuID = menu.ID, Name = "Çay & Kahve", IsRemovable = false }
                            );
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}