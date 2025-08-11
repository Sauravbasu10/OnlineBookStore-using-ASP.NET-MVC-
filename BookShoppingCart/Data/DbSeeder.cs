using BookShoppingCart.Constants;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookShoppingCart.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure roles exist
            if (!await roleMgr.RoleExistsAsync(Roles.Admin.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            if (!await roleMgr.RoleExistsAsync(Roles.User.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));


            //create admin user

            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userinDb = await userMgr.FindByEmailAsync(admin.Email);

            if(userinDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin@123");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
}
