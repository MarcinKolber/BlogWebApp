using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApp.Data
{
    public class SeedDb
    {
        // Method that seeds users and roles to the database
        // Code based on the C# source code by Medhat Elmasry
        // Accessed 6.01.2019
        // Source: https://github.com/medhatelmasry/IdentityCore/blob/master/IdentityCore/Data/DummyData.cs
        public static async Task SeedData(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminRole = "Admin";

            string customerRole = "Customer";

            string password = "Password123!";

            // Create roles
            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await roleManager.FindByNameAsync(customerRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(customerRole));
            }

            string adminAccount = "Member1@email.com";

            // Seed admin
            if (await userManager.FindByNameAsync(adminAccount) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminAccount,
                    Email = adminAccount,

                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }

            List<ApplicationUser> list = userManager.Users.ToList();

            // Seed customers
            for (int i = 1; i <= 5; i++)
            {
                string name = "Customer" + i + "@email.com";
                if (await userManager.FindByNameAsync(name) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = name,
                        Email = name,
                    };

                    var result = await userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.AddPasswordAsync(user, password);
                        await userManager.AddToRoleAsync(user, customerRole);
                    }
                }
            }
        }

    }
}
