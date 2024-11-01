using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // "Admin" ve "User" rolleri yoksa oluştur
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole<int>("User"));
        }

        // Admin kullanıcısını oluştur
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin",
                Email = "admin@hotmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // Kullanıcıyı "Password12+" şifresiyle oluştur
            var result = await userManager.CreateAsync(adminUser, "Password12+");
            if (result.Succeeded)
            {
                // Admin rolünü kullanıcıya ata
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
